using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace Datastore
{
    class MasterWorker : MarshalByRefObject, IMasterWorker
    {
        public void setAsReplica(Dictionary<int, string> availableServers)
        {
            Replica.ChangeToReplica(availableServers);
        }

        //TODO: Each time a worker registers, the master send the replica, if there is one
        // Replica url that worker uses maybe it should be placed in worker
        public void setReplica(string replica_url) 
        {
            Console.WriteLine("My replica is " + replica_url);
            Replica.REPLICAURL = replica_url;
        }
    }
}
