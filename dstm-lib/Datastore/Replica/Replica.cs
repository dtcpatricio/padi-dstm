using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace Datastore
{
    internal static class Replica
    {
        // Worker's replica url
        //private static string _replicaURL;

        // List of stored servers objects naturally orded from available servers in Master
        private static Dictionary<string, List<ServerObject>> worker_serverObjects = new Dictionary<string, List<ServerObject>>();

        private static string _sucessor;

        private static string _predecessor;



        internal static string SUCESSOR
        {
            get { return _sucessor; }
            set { _sucessor = value; }
        }

        internal static string PREDECESSOR
        {
            get { return _predecessor; }
            set { _predecessor = value; }
        }

        internal static Dictionary<string, List<ServerObject>> WORKERSERVEROBJECTS
        {
            get { return worker_serverObjects; }
        }
        /*  internal static void ChangeToReplica(Dictionary<int, string> availableServers)
          {
              Datastore.startReplicaMode(availableServers);         
          }
          */
        //TODO:  Substitute the datasore list of server objects with worker_serverObjects[id]
        internal static void changeToWorker(int id)
        {
            Datastore.SERVEROBJECTS = worker_serverObjects.ElementAt(id).Value;

            Console.WriteLine("SO LENGTH = " + worker_serverObjects.ElementAt(id).Value.Count);

            foreach (ServerObject s in worker_serverObjects.ElementAt(id).Value)
            {
                Console.WriteLine("UID = " + s.UID + " VALUE= " + s.VALUE);
            }

        }

        // Fetch the data from the servers and notify them that I am the Replica 
        /*   internal static void NotifyAllWorkers(Dictionary<int, string> availableServers)
           {
               foreach (int id in availableServers.Keys)
               {
                   IReplicaWorker worker = (IReplicaWorker)Activator.GetObject(
                       typeof(IReplicaWorker), availableServers[id] + "ReplicaWorker");
                   worker_serverObjects[availableServers[id]] = worker.fetchData(Datastore.SERVERURL);
               }
           }*/


        // Update function called by the workers, receives list of server objects 
        // from a given server. If worker_url does not exist in worker_serverObjects
        // it means that it is a new server

        //Send updated transaction written objects to replica if there is one
        internal static void updateSucessor(List<ServerObject> writtenObjects)
        {
           // try
            //{
                IWorkerReplica sucessor = (IWorkerReplica)Activator.GetObject(
                typeof(IWorkerReplica),
                Replica.SUCESSOR + "WorkerReplica");

                sucessor.update(Datastore.SERVERURL, writtenObjects);
            //}
            //catch (Exception e)
            //{
              //  Console.WriteLine(e.Message);
            //}
            /*
            // Create delegate to remote method
            RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(replica.update);
            // Call delegate to remote method
            Console.WriteLine("CALLING REPLICA TO UPDATE");
            IAsyncResult RemAr = RemoteDel.BeginInvoke(_serverURL, writtenObjects, null, null);
            Console.WriteLine("-- CALLING REPLICA TO UPDATE --");*/
        }

        // TODO: Test with sucessive updates 
        internal static void update(string predecessor, List<ServerObject> updatedList)
        {
            lock (worker_serverObjects)
            {
                if (!worker_serverObjects.ContainsKey(predecessor))
                {
                    worker_serverObjects.Add(predecessor, updatedList);

                    foreach (ServerObject so in worker_serverObjects[predecessor])
                    {
                        Console.WriteLine("\t" + "UPDATING UID=" + so.UID + " VALUE=" + so.VALUE);
                    }

                    return;
                }

                List<ServerObject> oldList = worker_serverObjects[predecessor];

                int j = 0;
                bool updated = false;
                foreach (ServerObject updatedSO in updatedList)
                {
                    foreach (ServerObject oldSO in oldList)
                    {
                        if (oldSO.UID.Equals(updatedSO.UID))
                        {
                            oldList[j] = updatedSO;
                            j = 0;
                            updated = true;
                            break;
                        }
                        j++;
                    }

                    if (updated == false)
                    {
                        oldList.Add(updatedSO);
                        updated = false;
                    }
                    j = 0;
                }
            }

            foreach (ServerObject so in worker_serverObjects[predecessor])
            {
                Console.WriteLine("\t" + "UPDATING UID=" + so.UID + " VALUE=" + so.VALUE);
            }
        }

        //TODO: Set sucessor
        internal static void setReplica(string sucessor, string predecessor)
        {
            SUCESSOR = sucessor;
            PREDECESSOR = predecessor;

            // Notify sucessor (sucessor, myURL -> predecessor)
            if (!sucessor.Equals(Datastore.SERVERURL) && (!predecessor.Equals(Datastore.SERVERURL)))
            {
                notifySucessor(sucessor, Datastore.SERVERURL);
                notifyPredecessor(predecessor, Datastore.SERVERURL);
            }
        }

        internal static void notifySucessor(string sucessor, string myURL)
        {
            IWorkerReplica replica = (IWorkerReplica)Activator.GetObject(
                    typeof(IWorkerReplica), sucessor + "WorkerReplica");
            replica.setPredecessor(myURL);
        }

        internal static void notifyPredecessor(string predecessor, string myURL)
        {
            IWorkerReplica replica = (IWorkerReplica)Activator.GetObject(
                    typeof(IWorkerReplica), predecessor + "WorkerReplica");

            worker_serverObjects[predecessor] = replica.setSucessor(myURL);
        }

    }
}
