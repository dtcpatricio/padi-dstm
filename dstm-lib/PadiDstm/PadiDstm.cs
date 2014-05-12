using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using CommonTypes;

namespace PADI_DSTM
{
    public static class PadiDstm
    {
        private static Transaction _transaction;
        private static ServersCache _servers;

        private static string _port;
        private static string _master_url;
        private static string _client_url;
        private static TcpChannel channel;

        internal static Transaction Transaction { get { return _transaction; } }
        internal static ServersCache Servers { get { return _servers; } }

        internal static string Client_Url { get { return _client_url; } }
        internal static string Master_Url { get { return _master_url; } }
        internal static string Port { get { return _port; } }


        public static bool Init()
        {
            System.Console.Write("Type in a port to use for EndTransaction object: ");
            _port = System.Console.ReadLine();
           // _port = "8090";
            _master_url = "tcp://localhost:8086/";
            _client_url = "tcp://localhost:" + _port + "/";

            // open a tcp channel to register the TransactionValues object
            // the port definition strategy needs to be taken into account
            channel = new TcpChannel(Convert.ToInt32(PadiDstm.Port));
            ChannelServices.RegisterChannel(channel, true);

            // register the EndTransaction object
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(EndTransaction),
                "EndTransaction",
                WellKnownObjectMode.SingleCall);

            _servers = new ServersCache();
            return true;
        }

        public static bool TxBegin()
        {
            _transaction = new Transaction(channel);

            // update the cache of data servers
            Servers.updateCache();

            return true; // placeholder
        }

        // Move Transaction.closeChannel to inside if (crossedLocks) 
        // Only if the transaction is committed we close the channel
        public static bool TxCommit()
        {
            if (Transaction.ACCESSEDSERVERS.Count > 0)
            {
                bool success = false;

                List<string> participants = Transaction.ACCESSEDSERVERS;
                string coordURL = participants.ElementAt(0);
                participants.RemoveAt(0);

                IDatastoreOps datastore = (IDatastoreOps)Activator.GetObject(
                typeof(IDatastoreOps),
                coordURL + "DatastoreOps");

                Console.WriteLine("PadiDstm.TxCommit: Before commit in Datastore");
                success = datastore.commit(Transaction.TXID, participants);
                Console.WriteLine("PadiDstm.TxCommit: After commit in Datastore");
                // TODO: there may be something missing still here

                _transaction = null;
                return success;
            }

            _transaction = null;
            //return false;
            return true;
        }

        public static bool TxAbort()
        {
            return true; // placeholder
        }


        /**
         * CreatePadInt
         * @param uid is a global unique identifier for the object in the system
         * @return PadInt an object representing to the client App what a PadInt is
         * 
         * CreatePadInt computes the server number based on the uid provided and the number
         * of servers available in the current transaction. It is guaranteed by the system
         * that the object with uid is in the server.
         **/
        public static PadInt CreatePadInt(int uid)
        {
            int serverNumber = computeDatastore(uid);

            string serverURL = Servers.AvailableServers[serverNumber];

            IDatastoreOps datastore = (IDatastoreOps)Activator.GetObject(
                typeof(IDatastoreOps),
                serverURL + "DatastoreOps");

            bool success = datastore.createPadInt(uid);
            if (success)
            {
                Console.WriteLine("PadiDstm.CreatePadInt TRUE");
                PadInt padint = new PadInt(uid, serverURL);
               
                return padint;
            }
            else
            {
                //Maybe throwexception unable to create padint, uid already exists
                return null;
            }
        }

        // returning -1 when 2 datastores are alive
        private static int computeDatastore(int uid)
        {
            // compute the digest of the uid
            byte[] uidBytes = BitConverter.GetBytes(uid);
            MD5 md5 = new MD5CryptoServiceProvider();
            
            byte[] digest = md5.ComputeHash(uidBytes);
            //string hash = getMd5Hash(digest);
            // get the first 4 bytes of the full 16 bytes of the hash
            // and have it moduled with the number of servers
            int test = BitConverter.ToInt32(digest, 0);
            int serverNumber = Math.Abs(test) % Servers.AvailableServers.Count;
            int test2 = Math.Abs(test);
            return serverNumber;
        }

        /**
         * AccessPadInt
         * @param uid is a global unique identifier for the object in the system
         * @return PadInt an object representing to the client App what a PadInt is
         * 
         * AccessPadInt computes the server number based on the uid provided and the number
         * of servers available in the current transaction. It is guaranteed by the system
         * that the object with uid is in the server.
         **/
        public static PadInt AccessPadInt(int uid)
        {
            int serverNumber = computeDatastore(uid);

            string serverURL = Servers.AvailableServers[serverNumber];

            IDatastoreOps datastore = (IDatastoreOps)Activator.GetObject(
                typeof(IDatastoreOps),
                serverURL + "DatastoreOps");

            bool success = datastore.accessPadInt(uid);
            if (success)
            {
                Console.WriteLine("PadiDstm.AccessPadInt TRUE");
                PadInt padint = new PadInt(uid, serverURL);
                return padint;
            }
            else
            {
                // Maybe throwexception unable to create padint, uid already exists
                // Enunciado: Returns null if the object does not exist already.
                return null;
            }
        }

        public static bool Status()
        {
            return true;
        }

        public static bool Fail(string url)
        {
            return true;
        }

        public static bool Freeze(string url)
        {
            return true;
        }

        public static bool Recover(string url)
        {
            return true;
        }
    }
}
