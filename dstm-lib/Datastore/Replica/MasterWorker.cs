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
                    IWorkerReplica sucessor = (IWorkerReplica)Activator.GetObject(typeof(IWorkerReplica), Replica.SUCESSOR + "WorkerReplica");
                    sucessor.update(Datastore.SERVERURL, replaceList);

                    Datastore.SERVEROBJECTS.AddRange(replaceList);
                    
                    // debug info
                    Console.WriteLine("Printing worker server objects:");
                    foreach (ServerObject o in Datastore.SERVEROBJECTS)
                    {
                        Console.WriteLine("\t UID= " + o.UID + " VALUE=" + o.VALUE);
                    }
                }

            }

        public void freeze()
        {
            Datastore.STATE = State.FREEZE;
        }
        public void recover()
        {
            Datastore.STATE = State.NORMAL;
        }
        public void fail()
        {
            Datastore.STATE = State.FAILED;
        }
    }
}
