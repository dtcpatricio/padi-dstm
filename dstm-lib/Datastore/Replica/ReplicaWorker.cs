using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace Datastore
{
    class ReplicaWorker : MarshalByRefObject, IReplicaWorker
    {
        //TODO: Sets the current datastore workers replica to allow communication
        public void setReplica(string url)
        {
            Console.WriteLine("Replica added with url: " + url);
            Replica.REPLICAURL = url;
        }
    }
}
