﻿using System;
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
        private static string _replicaURL;

        // List of stored servers objects naturally orded from available servers in Master
        private static Dictionary<string, List<ServerObject>> worker_serverObjects = new Dictionary<string, List<ServerObject>>();

        internal static string REPLICAURL
        {
            get { return _replicaURL; }
            set { _replicaURL = value; }
        }

        
        internal static void ChangeToReplica(Dictionary<int, string> availableServers)
        {
            Datastore.startReplicaMode(availableServers);         
        }

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
        internal static void NotifyAllWorkers(Dictionary<int, string> availableServers)
        {
            foreach (int id in availableServers.Keys)
            {
                IReplicaWorker worker = (IReplicaWorker)Activator.GetObject(
                    typeof(IReplicaWorker), availableServers[id] + "ReplicaWorker");
                worker_serverObjects[availableServers[id]] = worker.fetchData(Datastore.SERVERURL);
            }
        }


        // Update function called by the workers, receives list of server objects 
        // from a given server. If worker_url does not exist in worker_serverObjects
        // it means that it is a new server

        // TODO: Test with sucessive updates 
        internal static void update(string worker_url, List<ServerObject> updatedList)
        {
            lock (worker_serverObjects)
            {
                if (!worker_serverObjects.ContainsKey(worker_url))
                {
                    worker_serverObjects.Add(worker_url, updatedList);
                    return;
                }

                List<ServerObject> oldList = worker_serverObjects[worker_url];

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
        }
             
    }
}
