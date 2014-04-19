using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                int id = serverID++;
                availableServers.Add(id, url);
                Console.WriteLine("A Datastore Server was added on " + url + "\r\nwith ID " + id);
                return true;
            }
            else
                return false;
        }

        internal static IDictionary<int, string> getAvailableServers()
        {
            return availableServers;
        }
    }
}
