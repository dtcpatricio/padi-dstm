using System;
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
        // TODO: refactor to use generic list classes and HashSet
        // Map transaction id with its tentative reads/writes
        private static IDictionary<int, TentativeTx> _tentativeTransactions = new Dictionary<int, TentativeTx>();

        private static List<ServerObject> _serverObjects = new List<ServerObject>();

        //Hard coded master url
        private static string _masterURL = "tcp://localhost:8086/";

        private static string _serverURL;

        //timer for sending heartbeat
        private static Timer _timer;

        // Execution mode of server
        private static ExecutionMode _executionMode = ExecutionMode.WORKER;

        public delegate void RemoteAsyncDelegate(string serverURL, List<ServerObject> updatedSO);
        

        internal static string SERVERURL
        {
            get { return _serverURL; }
            set { _serverURL = value; }
        }

        internal static ExecutionMode EXECUTIONMODE
        {
            get { return _executionMode; }
            set { _executionMode = value; }
        }

        internal static List<ServerObject> SERVEROBJECTS
        {
            get { return _serverObjects; }
            set { _serverObjects = value; }
        }

        /**
         * createTentativeTx
         * @param txID the transaction id
         * @param clientURL the url of the client where this transaction is being executed
         * 
         * creates a tentative transaction in the datastore server
         **/
        internal static void createTentativeTx(int txID, string clientURL)
        {
            TentativeTx tx = new TentativeTx(txID, clientURL);
            _tentativeTransactions.Add(txID, tx);
        }
        /**
         * Registers a tentative READ
         * @return VOID
         * 
         * TODO: the method needs rework to make it efficient
         **/
        internal static int regTentativeRead(int uid, int txID, string clientURL)
        {
            if (!_tentativeTransactions.ContainsKey(txID))
                createTentativeTx(txID, clientURL);

            // We must ensure there's always a version for txID to read
            List<ServerObject> versions = getVersionsByUID(uid);
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
            return -1; // it should never reach this point. Possibly a TxException should be thrown
        }

        private static List<ServerObject> getVersionsByUID(int uid)
        {
            // TODO: lazy implementation - needs to change!
            List<ServerObject> versions = new List<ServerObject>();
            foreach (ServerObject obj in _serverObjects)
            {
                if (obj.UID == uid)
                    versions.Add(obj);
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

            List<ServerObject> versions = getVersionsByUID(uid);
            int earlierReadTS = versions.Max(readts => readts.READTS);

            if (earlierReadTS <= txID)
            {
                ServerObject tentativeWrite = new ServerObject(uid, newVal, txID);
                _serverObjects.Add(tentativeWrite);

                TentativeTx tx = _tentativeTransactions[txID];
                tx.AddObject(tentativeWrite);

                return true; // proceed with transaction
            }
            else
            {
                return false; // abort transaction
            }
        }

        /**
         * Creation and Access of server objects section
         **/
        internal static bool checkServerObject(int uid)
        {
            return _serverObjects.Any(x => x.UID == uid);
        }


        internal static bool createServerObject(int uid)
        {
            if(_serverObjects.Any((x => x.UID == uid && x.WRITETS == 0)))
                return false;

            ServerObject obj = new ServerObject(uid);
            _serverObjects.Add(obj);

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

        // Warning: Carefull with delays between sending "I Am Alive" e Master timer to check Heartbeat
        internal static void timerAlive()
        {
            // Create a timer with a twelve second interval.
            _timer = new Timer(12000);

            // Hook up the event handler for the Elapsed event.
            _timer.Elapsed += new ElapsedEventHandler(sendIAmAlive);

            // Only raise the event the first time Interval elapses.
            _timer.AutoReset = true;
            _timer.Enabled = true;
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
            Console.WriteLine("Datastore.Commit: Commit do Datastore");
            TentativeTx tx = _tentativeTransactions[txID];
            tx.COORDINATOR = new CoordinatorManager(tx, URLs);
            tx.COORDINATOR.prepare();
            if(tx.COORDINATOR.MY_DECISION.Equals(TransactionDecision.ABORT))
                return false; 
            
            return true;
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
        }

        internal static void doAbort(int txID, string coordURL)
        {
            TentativeTx tx = _tentativeTransactions[txID];
            tx.PARTICIPANT.doAbort(txID, coordURL);
        }

        // Change the mode of execution of datastore to replica
        internal static void startReplicaMode(Dictionary<int, string> availableServers)
        {
            EXECUTIONMODE = ExecutionMode.REPLICA;
            Replica.NotifyAllWorkers(availableServers);
        }


        //Send updated transaction written objects to replica if there is one
        internal static void updateReplica(List<ServerObject> writtenObjects)
        {
            if (Replica.REPLICAURL != null)
            {
                IWorkerReplica replica = (IWorkerReplica)Activator.GetObject(
                typeof(IWorkerReplica),
                Replica.REPLICAURL + "WorkerReplica");

                replica.update(_serverURL, writtenObjects);

                /*
                // Create delegate to remote method
                RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(replica.update);
                // Call delegate to remote method
                Console.WriteLine("CALLING REPLICA TO UPDATE");
                IAsyncResult RemAr = RemoteDel.BeginInvoke(_serverURL, writtenObjects, null, null);
                Console.WriteLine("-- CALLING REPLICA TO UPDATE --");*/
            }
        }
    }
}
