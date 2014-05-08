using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer
{
    class WorkerAlive : MarshalByRefObject, IWorkerAlive 
    {
        public void IAmAlive(String worker_url)
        {
            WorkerManager.IAmAlive(worker_url);
        }
    }
}
