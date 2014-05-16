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
            Console.WriteLine("Predecessor Worker: " + worker_url + " called update");
            Replica.update(writtenObjects);
        }

        public List<ServerObject> setSucessor(string sucessor)
        {
            Replica.SUCESSOR = sucessor;
            Console.WriteLine("NOW MY SUCESSOR IS = " + sucessor);
            return Datastore.SERVEROBJECTS;
        }

        public void setPredecessor(string predecessor)
        {
            Replica.PREDECESSOR = predecessor;
            Console.WriteLine("NOW MY PREDECESSOR IS = " + predecessor);
        }

        /**
         * Calls from Replica -> Datastore
         */
        public List<ServerObject> fetchData()
        {
            return Datastore.SERVEROBJECTS;
        }
    }
}
