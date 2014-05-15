using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer
{
    class LibraryComm : MarshalByRefObject, ILibraryComm
    {
        public int getTxID()
        {
            int txID = LibraryManager.getTxID();
            return txID;
        }

        public IDictionary<int, string> updateCache()
        {
            IDictionary<int, string> servers = WorkerManager.getAvailableServers();
            return servers;
        }

        public string setFailedServer(string failed_url)
        {
            Console.WriteLine("Setting failed server : " + failed_url);
            return WorkerManager.setFailedServer(failed_url);

        }



        // Freezes from the library. The master manages this situation
        public bool freeze(string url)
        {
            return WorkerManager.freeze(url);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
