using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace Padi_dstm
{
    public static class PadiDstm
    {
        // private bool txOpen = false; // placeholder
        private static Transaction _transaction;

        private static string _port;
        private static string _master_url;
        private static string _client_url;
        
        public static Transaction Transaction
        {
            get { return _transaction; }
        }

        public static string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public static string Client_Url
        {
            get { return _client_url; }
        }

        public static string Master_Url
        {
            get { return _master_url; }
        }

        public static bool Init()
        {
            System.Console.Write("Type in a port to use for TransactionValues object: ");
            string port = System.Console.ReadLine();
            PadiDstm.Port = port;
            PadiDstm._master_url = "tcp://localhost:8086/WorkerRegister";
            _client_url = "tcp://localhost:" + PadiDstm.Port + "/TransactionValues";
            return true;
        }

        public static bool TxBegin()
        {
            _transaction = new Transaction();
            return true; // placeholder
        }

        public static bool TxCommit()
        {
            // Check in the master server if the current transaction
            // has performed any conflicting operations
            return true; // placeholder
        }

        public static bool TxAbort()
        {
            return true; // placeholder
        }

        public static bool Status()
        {
            // TODO: implement class Status presented in CommonTypes
            return true;
        }

        public static bool Fail(string url)
        {
            // In context of a transaction?
            return _transaction.Fail(url); ;
        }

        public static bool Freeze(string url)
        {
            // In context of a transaction?
            return _transaction.Freeze(url); ;
        }

        public static bool Recover(string url)
        {
            // In context of a transaction?
            return _transaction.Recover(url); ;
        }

        public static PadInt CreatePadInt(int uid)
        {
            IWorkerRegister remote = (IWorkerRegister)Activator.GetObject(
                typeof(IWorkerRegister),
                PadiDstm._master_url);

            Console.WriteLine("Master Url: " + _master_url);
            Console.Write("remote : " + remote);

            bool isCreated = remote.createPadIntMaster(_transaction.getTransactionId(), uid, _client_url);
            if (isCreated)
            {
                Console.WriteLine("PadiDstm.CreatePadInt TRUE");
                PadInt padint = new PadInt(uid);
                return padint;
            }
            else
            {
                //Maybe throwexception unable to create padint, uid already exists
                return null;
            }
        }

        public static PadInt AccessPadInt(int uid)
        {
             // placeholder values for testing
            PadInt padint = new PadInt(uid);
            return padint;
        }
    }
}
