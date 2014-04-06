using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using padi_dstm_library;

namespace WorkerServer
{
    class PadIntWorker : MarshalByRefObject, Library
    {
        // Key: int uid
        // Value: PadInt
        private IDictionary<int, PadInt> padInts;

        public PadIntWorker()
        {
            padInts = new Dictionary<int, PadInt>();
        }

        public PadInt CreatePadInt(int uid)
        {
            PadInt pi = new PadInt(uid, "tcp://localhost:8087/Server/");

            AddPadInt(uid, pi);
            Console.WriteLine("PadInt Created with uid: " + uid + ".");
            return pi;
        }

        public void AddPadInt(int uid, PadInt pi)
        {
            padInts.Add(uid, pi);
        }

        public PadInt AccessPadInt(int uid)
        {
            Console.WriteLine("PadInt Accessed with uid: " + uid + ".");
            return padInts[uid];
        }
    }
}
