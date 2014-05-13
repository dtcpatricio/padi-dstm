﻿using System;
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

        public void setFailedServer(string failed_url)
        {
            Console.WriteLine("Setting failed server : " + failed_url);
            WorkerManager.setFailedServer(failed_url);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
