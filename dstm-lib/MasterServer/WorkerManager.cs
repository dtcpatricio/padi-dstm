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

        // Returns the total number of servers including the replica and failed servers
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
                if (totalServers() != 1)
                {
                    int id = serverID++;
                    availableServers.Add(id, url);
                    SetReplica(url);
                    HeartBeat.TIMERSERVERS.Add(url, HeartBeat.timerAlive(url));
                    Console.WriteLine("A Datastore Server was added on " + url + "\r\nwith ID " + id);
                    return true;
                }
                else
                {
                    CreateReplica(url);
                    HeartBeat.TIMERSERVERS.Add(url, HeartBeat.timerAlive(url));
                    Console.WriteLine("A Datastore Replica was created on " + url + "\r\n");
                    return true;
                }
            }
            else
                return false;
        }

        // Send a a setAsReplica for the url worker
        internal static void CreateReplica(string url)
        {
            IMasterWorker remote = (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                url + "MasterWorker");
            remote.setAsReplica(availableServers);

            REPLICAURL = url;
        }

        // Set the replica for the worker if there is one
        internal static void SetReplica(string url)
        {
            if (REPLICAURL != null)
            {
                IMasterWorker remote = (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                    url + "MasterWorker");
                remote.setReplica(REPLICAURL);
            }
        }

        // Sets the detected url to failed
        internal static void setFailedServer(string url)
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
                    return;
                }
            }
        }

    }
}
