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
        /*public List<ServerObject> fetchData(string url)
        {
            Console.WriteLine("Replica added with url: " + url);
            Replica.REPLICAURL = url;
            return Datastore.SERVEROBJECTS;
        }*/
    }
}
