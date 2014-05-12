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


        static private String _replicaURL = null;

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
                        return availableServers.ElementAt(availableServers.Count-1).Value;
                    }
                    return availableServers.ElementAt(i - 1).Value;
                }
            }

            // Should never reach this path
            return null;
        }
         

        internal static String REPLICAURL
        {
            get { return _replicaURL; }
            set { _replicaURL = value; }
        }

        internal static IDictionary<int, string> getAvailableServers()
        {
            return availableServers;
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

        // Verifies if the replica has been createad
        internal static bool ReplicaExists()
        {
            if (REPLICAURL != null)
                return true;
            return false;
        }

        // Returns the total number of servers incl94e0uding the replica and failed servers
        internal static int totalServers()
        {
            int total = 0;
            if (REPLICAURL != null)
                total++;
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
       /* internal static void setFailedServer(string url)
        {
            if (REPLICAURL != null && REPLICAURL.Equals(url))
            {
                REPLICAURL = null;
                return;
            }
            foreach (int id in availableServers.Keys)
            {
                if (availableServers[id].Equals(url))
                {
                    failedServers.Add(id, url);
                    availableServers.Remove(id);

                    //TODO: Tell replica to replace this failed server
                    setWorker(id);
                    return;
                }
            }
        }*/

       /* internal static void setWorker(int id)
        {
            IMasterWorker remote = (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                    REPLICAURL + "MasterWorker");

            remote.setWorker(id);
        }
        */
    }
}
