﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting;
using System.IO;
using CommonTypes;

namespace Datastore
{
    [Serializable]
    internal static class Datastore
    {
        // Map transaction id with its tentative reads/writes
        private static IDictionary<int, TentativeTx> _tentativeTransactions = new Dictionary<int, TentativeTx>();

        private static List<ServerObject> _serverObjects = new List<ServerObject>();

        //Hard coded master url
        private static string _masterURL = "tcp://localhost:8086/";

        private static string _serverURL;

        // Normal, Failed or Freezed
        private static State _state;

        public delegate void RemoteAsyncDelegate(string serverURL, List<ServerObject> updatedSO);


        internal static string MASTER { get { return _masterURL; } }

        internal static string SERVERURL
        {
            get { return _serverURL; }
            set { _serverURL = value; }
        }

        internal static State STATE
        {
            get { return _state; }
            set { _state = value; }
        }

        internal static List<ServerObject> SERVEROBJECTS
        {
            get { return _serverObjects; }
            set { _serverObjects = value; }
        }

        internal static void Freeze()
        {
            while (_state.Equals(State.FREEZE))
            {
                // Loop until some client calls recover
                // or the library tells the master to kill the datastore
            }
        }

        /**
         * createTentativeTx
         * @param txID the transaction id
         * @param clientURL the url of the client where this transaction is being executed
         * 
         * creates a tentative transaction in the datastore server
         **/
        internal static TentativeTx createTentativeTx(int txID, string clientURL)
        {
            TentativeTx tx = new TentativeTx(txID, clientURL);
            _tentativeTransactions.Add(txID, tx);
            return tx;
        }

        internal static List<ServerObject> getVersionsRead(List<ServerObject> serverObjects, int uid)
        {
            try
            {
                List<ServerObject> versions = new List<ServerObject>();
                lock (serverObjects)
                {
                    foreach (ServerObject so in serverObjects)
                    {
                        if (so.UID == uid)
                        {
                            versions.Add(so);
                        }
                    }
                }

                return versions;
            }
            catch (Exception)
            {
                Console.WriteLine("Catch CALLING READTE!");
                return null;
            } 
        }

        /**
         * Registers a tentative READ
         * @return VOID
         * 
         * TODO: the method needs rework to make it efficient
         **/
        internal static int regTentativeRead(int uid, int txID, string clientURL)
        {
            try
            {
                if (!_tentativeTransactions.ContainsKey(txID))
                    createTentativeTx(txID, clientURL);

                TentativeTx tx = _tentativeTransactions[txID];

                // We must ensure there's always a version for txID to read
                List<ServerObject> versions = getVersionsRead(tx.WRITTENOBJECTS, uid);

                versions.Sort((x, y) => x.WRITETS.CompareTo(y.WRITETS));
                versions.Reverse();
                foreach (ServerObject obj in versions)
                {
                    if (obj.WRITETS <= txID)
                    {
                        obj.READTS = txID;
                        return obj.VALUE;
                    }
                }

                // Only comes here if it's not in written objects of this tx
                versions = getVersionsRead(_serverObjects, uid);

                versions.Sort((x, y) => x.WRITETS.CompareTo(y.WRITETS));
                versions.Reverse();
                foreach (ServerObject obj in versions)
                {
                    if (obj.WRITETS <= txID)
                    {
                        obj.READTS = txID;
                        return obj.VALUE;
                    }
                }
                return -1; // It should never reach this point
            }
            catch (Exception)
            {
                Console.WriteLine("Catch CALLING WRITE!");
                return -1;
            } 

        }

        private static List<ServerObject> getVersionsByUID(int uid, int txID)
        {
            TentativeTx tx = _tentativeTransactions[txID];
            bool versionWritten = false;
            List<ServerObject> versions = new List<ServerObject>();
            
            foreach (ServerObject obj in tx.WRITTENOBJECTS)
            {
                if (obj.UID == uid)
                {
                    versions.Add(obj);
                    versionWritten = true;
                }
            }
            if (versionWritten)
            {
                return versions;
            }
            lock (_serverObjects)
            {
                foreach (ServerObject obj in _serverObjects)
                {
                    if (obj.UID == uid)
                        versions.Add(obj);
                }
            }
            return versions;
        }

        /**
         * Registers a tentative Write
         * @return TRUE if the value could be written
         * @return FALSE if the write is in conflict, cascading into an abort Tx
         **/
        internal static bool regTentativeWrite(int uid, int newVal, int txID, string clientURL)
        {
            if (!_tentativeTransactions.ContainsKey(txID))
                createTentativeTx(txID, clientURL);

            List<ServerObject> versions = getVersionsByUID(uid, txID);
            int earlierReadTS = versions.Max(readts => readts.READTS);

            if (earlierReadTS <= txID)
            {
                ServerObject tentativeWrite = new ServerObject(uid, newVal, txID);

                TentativeTx tx = _tentativeTransactions[txID];
                tx.AddObject(tentativeWrite);

                return true; // proceed with transaction
            }
            else
            {
                return false; // abort transaction
            }

        }

        internal static void addValues(List<ServerObject> serverObjects)
        {
            lock (_serverObjects)
            {
                _serverObjects.AddRange(serverObjects);
            }
        }


        /**
         * Creation and Access of server objects section
         **/
        internal static bool checkServerObject(int uid)
        {
            return _serverObjects.Any(x => x.UID == uid);
        }


        internal static bool createServerObject(int uid, int txID, string clientURL)
        {
            if (_serverObjects.Any((x => x.UID == uid && x.WRITETS == 0)))
                return false;

            TentativeTx tx;
            if (!_tentativeTransactions.ContainsKey(txID))
            {
                tx = createTentativeTx(txID, clientURL);
            }
            else
            {
                tx = _tentativeTransactions[txID];
            }

            ServerObject obj = new ServerObject(uid);
            tx.AddObject(obj);
            return true;
        }

        // TODO: Send heartbeat to Master server
        internal static void sendIAmAlive(object source, ElapsedEventArgs e)
        {

            // register the datastore server on the master server
            IWorkerAlive master = (IWorkerAlive)Activator.GetObject(
                typeof(IWorkerAlive),
                _masterURL + "WorkerAlive");

            master.IAmAlive(SERVERURL);

        }

        /**********************************************************************
         * 2PC protocol section
         * TODO: Maybe it could be in another section (like a different class)
         **********************************************************************/

        /**********************************************************************
         * COORDINATOR part
         **********************************************************************/

        // This method is only called by the coordinator
        // URLs is the list of participants of transaction txID
        internal static bool Commit(int txID, List<String> URLs)
        {
            try
            {
                Console.WriteLine("Datastore.Commit: Commit do Datastore");
                TentativeTx tx = _tentativeTransactions[txID];
                tx.COORDINATOR = new CoordinatorManager(tx, URLs);
                tx.COORDINATOR.prepare();
                if (tx.COORDINATOR.MY_DECISION.Equals(TransactionDecision.ABORT))
                    return false;

                addValues(tx.WRITTENOBJECTS);

                // Send an update to the replica if there is one
                if (Replica.updateSucessor(tx.WRITTENOBJECTS).Equals(UpdateState.COMMIT))
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                Console.WriteLine("CATCH IN COMMIT");
                return false;
            }
        }

        internal static void participantVoteYes(int txID, String URL)
        {

            TentativeTx tx = _tentativeTransactions[txID];
            tx.COORDINATOR.participantYes(URL);

        }

        internal static void participantVoteNo(int txID, String URL)
        {

            TentativeTx tx = _tentativeTransactions[txID];
            tx.COORDINATOR.participantNo(URL);

        }

        internal static bool haveCommitted(int txID, String url)
        {
            TentativeTx tx = _tentativeTransactions[txID];
            return tx.COORDINATOR.haveCommitted(url);
        }

        /***********************************************************************
         * PARTICIPANT part
         ***********************************************************************/

        /**
         * canCommit
         * @param txID id of the transaction to commit
         * @coordURL url of the coordinator server in the protocol
         * 
         * this method is called from coordinator to participants to commit their values
         * and reply to the coordinator with an answer of success
         **/
        internal static void canCommit(int txID, string coordURL)
        {
            TentativeTx tx = _tentativeTransactions[txID];
            tx.PARTICIPANT = new ParticipantManager(tx);
            tx.PARTICIPANT.COORDINATORURL = coordURL;
            tx.PARTICIPANT.canCommit();
        }

        internal static void doCommit(int txID, string coordURL)
        {
            TentativeTx tx = _tentativeTransactions[txID];
            tx.PARTICIPANT.doCommit(txID, coordURL);
            // Send an update to the replica if there is one
            Console.WriteLine("MY OBJECTS ARE : ");

            //replaceValue(tx.WRITTENOBJECTS);
            addValues(tx.WRITTENOBJECTS);
            /*
            foreach (ServerObject o in SERVEROBJECTS)
            {
                Console.WriteLine("\t UID= " + o.UID + " VALUE=" + o.VALUE);
            }
            */
            Replica.updateSucessor(tx.WRITTENOBJECTS);
        }

        internal static void doAbort(int txID, string coordURL)
        {
            TentativeTx tx = _tentativeTransactions[txID];
            tx.PARTICIPANT.doAbort(txID, coordURL);
        }
    }
}
