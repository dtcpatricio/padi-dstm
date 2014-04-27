﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    internal static class Datastore
    {

        // TODO: refactor to use generic list classes and HashSet
        private static IDictionary<int, TentativeTx> _tentativeTransactions = new Dictionary<int, TentativeTx>();

        private static List<ServerObject> _serverObjects = new List<ServerObject>();

        // maybe it should not be here
        private static string _serverURL;

        // Transaction Manager global test
        private static TransactionManager _tm;

        internal static TransactionManager TRANSACTIONMANAGER
        {
            get { return _tm; }
        }

        internal static string SERVERURL
        {
            get { return _serverURL; }
            set { _serverURL = value; }
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


        /**
         * 2PC protocol section
         * TODO: Maybe it could be in another section (like a different class)
         */
        internal static bool Commit(int txID, List<String> URLs)
        {
            TentativeTx tx = _tentativeTransactions[txID];
            _tm = new TransactionManager(tx, URLs);
            _tm.addTransactionURLs(tx, URLs);
            _tm.prepare();

            return true;
        }

        /**
         * canCommit
         * @param txID id of the transaction to commit
         * @coordURL url of the coordinator server in the protocol
         * 
         * this method is called by the participants to commit their values
         * and reply to the coordinator with an answer of success
         **/
        internal static void canCommit(int txID, string coordURL)
        {
            TentativeTx tx = _tentativeTransactions[txID];
            _tm = new TransactionManager(tx);
            _tm.addTransaction(tx);
            ICoordinator coord = (ICoordinator)Activator.GetObject(typeof(ICoordinator), coordURL);
            if (_tm.canCommit())
                coord.sendYes(txID, SERVERURL);
            else
                coord.sendNo(txID, SERVERURL);

        }

        // Only for participants
        internal static void DoCommit(int txID, string coordURL)
        {
            // changes of tentative tx made permanent
        }

        // Only for participants
        internal static void DoAbort(int txID, string coordURL)
        {
            // delete tentative tx
        }
    }
}
