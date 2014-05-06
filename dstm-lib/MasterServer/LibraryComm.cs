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

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
