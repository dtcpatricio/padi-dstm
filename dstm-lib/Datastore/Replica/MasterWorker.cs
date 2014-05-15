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
       /* public void setAsReplica(Dictionary<int, string> availableServers)
        {
            Replica.ChangeToReplica(availableServers);
        }

        // The master ID of the worker server to replace
        public void setWorker(int id)
        {
            Console.WriteLine("NOW IM A WORKER WITH ID=" + id);
            Replica.changeToWorker(id);
        }
        */
        // The master ID of the worker server to replace
        public void setReplica(string sucessor, string predecessor)
        {
            Console.WriteLine("NOW MY SUCESSOR IS = " + sucessor + " PREDECESSOR IS = " + predecessor);
            Replica.setReplica(sucessor, predecessor);
        }

        // The worker has a new sucessor
        public void setSucessor(string sucessor)
        {
            Console.WriteLine("AFTER SERVER FAILED MY SUCESSOR IS = " + sucessor);
            Replica.SUCESSOR = sucessor;
        }

        // The worker has a new sucessor
        public void setPredecessor(string predecessor)
        {
            Console.WriteLine("AFTER SERVER FAILED MY PREDECESSOR IS = " + predecessor);
            Replica.PREDECESSOR = predecessor;
        }

        //TODO: Start receiving requests from the failed server
        public void substituteFailedServer(string failed_server)
        {
            lock (Datastore.SERVEROBJECTS)
            {   // If false, the server has not received any update from failed server
                if (Replica.WORKERSERVEROBJECTS.ContainsKey(failed_server))
                {
                    List<ServerObject> replaceList = Replica.WORKERSERVEROBJECTS[failed_server];

                    //Send replaceList to sucessor, the data must always be in two place
                    IWorkerReplica sucessor = (IWorkerReplica)Activator.GetObject(
                              typeof(IWorkerReplica),
                    Replica.SUCESSOR + "WorkerReplica");

                    sucessor.update(Datastore.SERVERURL, replaceList);

                    //void update(string worker_url, List<ServerObject> writtenObjects);

                    foreach (ServerObject so in replaceList)
                    {
                        Console.WriteLine("Adding server object from failed server with uid= " + so.UID);
                        Datastore.SERVEROBJECTS.Add(so);
                    }

                    Console.WriteLine("Printing worker server objects:");
                    foreach (ServerObject o in Datastore.SERVEROBJECTS)
                    {
                        Console.WriteLine("\t UID= " + o.UID + " VALUE=" + o.VALUE);
                    }
                }

            }

        }

        public void freeze()
        {
            Datastore.STATE = State.FREEZE;
        }
    }
}
