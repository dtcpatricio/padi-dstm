using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.DatastoreMaster;

namespace MasterServer
{
    class DatastoreComm : MarshalByRefObject, IDatastoreComm
    {
        public bool registerWorker(string url)
        {
            bool success = WorkerManager.addServer(url);
            return success;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
