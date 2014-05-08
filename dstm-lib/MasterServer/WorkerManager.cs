using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer
{
    internal static class WorkerManager
    {
        // Identifiers for servers
        static private int serverID = 0;

        // Maps uid with worker references
        //static private IDictionary<int, string> padIntUids = new Dictionary<int, string>();

        // List of available workers
        static private IDictionary<int, string> availableServers = new Dictionary<int, string>();

        // List of failed workers
        static private IDictionary<int, string> failedServers = new Dictionary<int, string>();

        static private String _replicaURL = null;


        internal static String REPLICAURL 
        {
            get { return _replicaURL; }
            set { _replicaURL = value; }
        }

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

        internal static bool addServer(string url)
        {
            if (!availableServers.Values.Contains(url) &&
                !failedServers.Values.Contains(url))
            {
                if (ReplicaExists())
                {
                    int id = serverID++;
                    availableServers.Add(id, url);
                    Console.WriteLine("A Datastore Server was added on " + url + "\r\nwith ID " + id);
                    return true;
                }
                else
                {
                    CreateReplica(url);
                    return true;
                }
            }
            else
                return false;
        }

        
        // Verifies if the replica has been createad
        internal static bool ReplicaExists()
        {
            if(REPLICAURL != null) 
                return true;
            return false;
        }

        // Send a a setAsReplica for the url worker
        internal static void CreateReplica(string url)
        {
            IMasterWorker remote = (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                url + "MasterWorker");
            remote.setAsReplica(availableServers);
        }

        internal static IDictionary<int, string> getAvailableServers()
        {
            return availableServers;
        }
    }
}
