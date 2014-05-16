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
        // The master ID of the worker server to replace
        public void setReplica(string sucessor, string predecessor)
        {
            Console.WriteLine("NOW MY SUCESSOR IS = " + sucessor + " PREDECESSOR IS = " + predecessor);
            Replica.setReplica(sucessor, predecessor);
        }

        // The worker has a new sucessor (sends updates to a new one)
        public void setSucessor(string sucessor)
        {
            Console.WriteLine("AFTER SERVER FAILED MY SUCESSOR IS = " + sucessor);
            Replica.SUCESSOR = sucessor;
        }

        // WARNING: This function is obsolete

        // The worker has a new predecessor (receives updates from a new one)
        public void setPredecessor(string predecessorURL)
        {
            IWorkerReplica predecessor = (IWorkerReplica)Activator.GetObject(typeof(IWorkerReplica), predecessorURL + "WorkerReplica");
            List<ServerObject> backupObjects = predecessor.fetchData();
            Replica.update(backupObjects);
            Console.WriteLine("AFTER SERVER FAILED MY PREDECESSOR IS = " + predecessor);
            Replica.PREDECESSOR = predecessorURL;
        }

        //TODO: Start receiving requests from the failed server
        public void substituteFailedServer()
        {
            lock (Datastore.SERVEROBJECTS)
            {
                List<ServerObject> replaceList = Replica.WORKERSERVEROBJECTS;

                //Send replaceList to sucessor, the data must always be in two place
                // WARNING: Verificar se o sucessor é ele próprio
                IWorkerReplica sucessor = (IWorkerReplica)Activator.GetObject(
                    typeof(IWorkerReplica), Replica.SUCESSOR + "WorkerReplica");
                

                Datastore.SERVEROBJECTS.AddRange(replaceList);

                
                //sucessor.update(replaceList);

                // Instead of sending only the replace list, it is send all primary objects just to be sure
                sucessor.update(Datastore.SERVEROBJECTS);

                // debug info
                Console.WriteLine("Printing worker main server objects:");
                foreach (ServerObject o in Datastore.SERVEROBJECTS)
                {
                    Console.WriteLine("\t UID= " + o.UID + " VALUE=" + o.VALUE);
                }
            }

        }

        // This server will fetch all primary data of sucessor and add it in his list of updates
        // WARNING: Verificar o caso que o sucessor é ele próprio
        public void fetch_data(string predecessor_url)
        {
            IWorkerReplica predecessor = (IWorkerReplica)Activator.GetObject(
                    typeof(IWorkerReplica), predecessor_url + "WorkerReplica");

            List<ServerObject> fetched_data = predecessor.fetchData();

            // WARNING: Será que existe algum caso em que adiciona dados repetidos?
            // Como os dados existem sempre em 2 sitios deve estar correcto
            Replica.WORKERSERVEROBJECTS = fetched_data;
        }

        // This server will fetch all primary data of sucessor and replace its own primary data
        public void fetch_recover_data(string sucessor_url)
        {
            IWorkerReplica sucessor = (IWorkerReplica)Activator.GetObject(
                    typeof(IWorkerReplica), sucessor_url + "WorkerReplica");

            List<ServerObject> recovered_data = sucessor.fetchRecoverData();

            Replica.SUCESSOR = sucessor_url;
            Datastore.SERVEROBJECTS = recovered_data;
            Datastore.STATE = State.NORMAL;
            Console.WriteLine("Datastore Recovered");
        }

        public void status()
        {
            Console.WriteLine("");
            Console.WriteLine("**** STATUS ****");
            foreach (ServerObject so in Datastore.SERVEROBJECTS)
            {
                Console.WriteLine("\t " + "UID=" + so.UID + " VALUE=" + so.VALUE); 
            }
            Console.WriteLine("**** REPLICATED ****");
            
            foreach (ServerObject so in Replica.WORKERSERVEROBJECTS)
            {
                Console.WriteLine("\t " + "UID=" + so.UID + " VALUE=" + so.VALUE);
            }

            Console.WriteLine("**** END ****");
            Console.WriteLine("");
        }

        public void freeze()
        {
            Console.WriteLine("Worker Freezed");
            Datastore.STATE = State.FREEZE;
        }
        public void recover()
        {
            Console.WriteLine("Worker Recovered");
            Datastore.STATE = State.NORMAL;
        }
        public void fail()
        {
            Console.WriteLine("Worker Failed");
            Datastore.STATE = State.FAILED;
        }
    }
}
