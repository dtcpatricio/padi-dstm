using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Timers;

namespace MasterServer
{
    // Consider : Removing all serverID, let the server be identified by the url
    internal static class WorkerManager
    {
        // Identifiers for servers
        static private int serverID = 0;

        // Maps uid with worker references
        //static private IDictionary<int, string> padIntUids = new Dictionary<int, string>();

        // List of available workers
        static private Dictionary<int, string> availableServers = new Dictionary<int, string>();

        // List of failed workers
        static private Dictionary<int, string> failedServers = new Dictionary<int, string>();

        // List of freezed workers
        static private Dictionary<int, string> freezedServers = new Dictionary<int, string>();

        /*
        static public void printAvailableWorkers()
        {
            Console.WriteLine("Available workers: ");
            for (int i = 0; i < getWorkers().Count; i++)
            {
                Console.WriteLine("\t" + getAvailableWorker(i) + ".");
            }
        }
        */

        internal static IDictionary<int, string> getAvailableServers()
        {
            return availableServers;
        }

        // Returns the id sucessor in replication chain
        internal static string getWorkerSucessor(int id)
        {
            for (int i = 0; i < availableServers.Count; i++)
            {
                if (availableServers.ElementAt(i).Key == id)
                {
                    if (i + 1 == availableServers.Count)
                    {
                        return availableServers.ElementAt(0).Value;
                    }
                    return availableServers.ElementAt(i + 1).Value;
                }
            }

            // Should never reach this path
            return null;
        }


        // Returns the id predecessor in replication chain
        internal static string getWorkerPredecessor(int id)
        {
            for (int i = 0; i < availableServers.Count; i++)
            {
                if (availableServers.ElementAt(i).Key == id)
                {
                    if (i == 0)
                    {
                        return availableServers.ElementAt(availableServers.Count - 1).Value;
                    }
                    return availableServers.ElementAt(i - 1).Value;
                }
            }
            // Should never reach this path
            return null;
        }

        internal static string getFailedSucessor(int failed_id)
        {
            // Return the sucessor of the failed server
            if (availableServers.ContainsKey(failed_id + 1))
                return availableServers[failed_id+1];

            // Return the head of the chain
            if (availableServers.Count > 0)
                return availableServers.ElementAt(0).Value;

            return null;
        }

        internal static bool isFailedServer(string url)
        {
            foreach (int id in failedServers.Keys)
            {
                if (failedServers[id].Equals(url))
                    return true;
            }
            return false;
        }

        internal static bool isFreezedServer(string url)
        {
            foreach (int id in freezedServers.Keys)
            {
                if (freezedServers[id].Equals(url))
                    return true;
            }
            return false;
        }

        // Master tells the worker to freeze itself
        internal static bool freeze(string url)
        {
            if (isFailedServer(url) || isFreezedServer(url)) return false;

            IMasterWorker worker = (IMasterWorker)Activator.GetObject(
                typeof(IMasterWorker), url + "MasterWorker");
            worker.freeze();

            return true;
        }

        internal static bool recover(string url)
        {
            return true;
        }

        // Returns the total number of servers incl94e0uding the replica and failed servers
        internal static int totalServers()
        {
            int total = 0;
            return availableServers.Count + failedServers.Count + total;
        }


        // Second Server added is always the replica server
        // TODO: FIX UGLY CODE
        internal static bool addServer(string url)
        {
            if (!availableServers.Values.Contains(url) &&
                !failedServers.Values.Contains(url))
            {
                int id = serverID++;
                availableServers.Add(id, url);
                SetReplica(url, getWorkerSucessor(id), getWorkerPredecessor(id));
                Console.WriteLine("A Datastore Server was added on " + url + "\r\nwith ID " + id);
                return true;
            }
            else
                return false;
        }

        // Send a a setAsReplica for the url worker
        /*   internal static void CreateReplica(string url)
           {

               REPLICAURL = url;
               IMasterWorker remote = (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                   url + "MasterWorker");
               remote.setAsReplica(availableServers);

           }*/

        // Set the sucessor replica of the worker
        internal static void SetReplica(string worker_url, string sucessor, string predecessor)
        {
            IMasterWorker remote = (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                worker_url + "MasterWorker");
            remote.setReplica(sucessor, predecessor);
        }

        // Sets the detected url to failed
        internal static string setFailedServer(string failed_url)
        {
            lock (availableServers)
            {
                foreach (int id in availableServers.Keys)
                {
                    if (availableServers[id].Equals(failed_url))
                    {
                        // Get the sucessor of failed server and
                        // set the predecessor's sucessor to this server
                        string failed_sucessor = getWorkerSucessor(id);
                        string failed_predecessor = getWorkerPredecessor(id);

                        IMasterWorker remote_sucessor =
                                        (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                                          failed_sucessor + "MasterWorker");

                        remote_sucessor.setPredecessor(failed_predecessor);

                        IMasterWorker remote_predecessor =
                                        (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                                          failed_predecessor + "MasterWorker");
                        
                        remote_predecessor.setSucessor(failed_sucessor);
                        remote_sucessor.substituteFailedServer(failed_url);

                        failedServers.Add(id, failed_url);
                        availableServers.Remove(id);

                        return failed_sucessor;
                    }
                }
                // TESTAR ESTE CASO 2 CLIENTES
                foreach (int id in failedServers.Keys)
                {
                    if(failedServers[id].Equals(failed_url))
                        return getFailedSucessor(id);
                }
                Console.WriteLine("WorkerManager.setFailedServers IT SHOULD NOT BE HERE");
                return null;
            }
        }

        /* internal static void setWorker(int id)
         {
             IMasterWorker remote = (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                     REPLICAURL + "MasterWorker");

             remote.setWorker(id);
         }
         */
    }
}
