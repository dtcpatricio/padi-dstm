using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

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


        // The master ID of the worker server to replace
        public void setWorker(int id)
        {
            Console.WriteLine("NOW IM A WORKER WITH ID=" + id);
            Replica.changeToWorker(id);
        }
    }
}
