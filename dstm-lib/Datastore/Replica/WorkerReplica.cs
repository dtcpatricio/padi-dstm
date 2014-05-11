using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    class WorkerReplica : MarshalByRefObject, IWorkerReplica
    {
        public void update(string worker_url, List<ServerObject> writtenObjects)
        {
            Console.WriteLine("CALLED INSIDE REPLICA");
            Replica.update(worker_url, writtenObjects);
        }
    }
}
