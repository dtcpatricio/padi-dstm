using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Padi_dstm
{
    public static class PadiDstm
    {
        // private bool txOpen = false; // placeholder
        private static Transaction _transaction;

        private static string _port;

        public static Transaction Transaction
        {
            get { return _transaction; }
        }
        public static string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public static bool Init()
        {
            System.Console.Write("Type in a port to use for TransactionValues object: ");
            string port = System.Console.ReadLine();
            PadiDstm.Port = port;
            return true;
        }
        public static bool TxBegin()
        {
            _transaction = new Transaction();
            return true; // placeholder
        }

        public static bool TxCommit()
        {
            return true; // placeholder
        }

        public static bool TxAbort()
        {
            return true; // placeholder
        }

        public static PadInt CreatePadInt(int uid)
        {
            return null; // placeholder
        }

        public static PadInt AccessPadInt(int uid)
        {
             // placeholder values for testing
            PadInt padint = new PadInt(uid);
            return padint;
        }
    }
}
