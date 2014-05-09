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

        //timer
        private static Timer _timer;


        internal static Timer TIMER
        {
            get { return _timer; }
            set { _timer = value; }
        }

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
                if (availableServers.Count != 1)
                {
                    int id = serverID++;
                    availableServers.Add(id, url);
                    Console.WriteLine("A Datastore Server was added on " + url + "\r\nwith ID " + id);
                    return true;
                }
                else
                {
                    CreateReplica(url);
                    Console.WriteLine("A Datastore Replica was created on " + url + "\r\n");
                    return true;
                }
            }
            else
                return false;
        }

        internal static int totalServers()
        {
            return availableServers.Count + failedServers.Count;
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

            REPLICAURL = url;
        }

        // TODO: reset timer for the specified worker_url
        internal static void IAmAlive(String worker_url)
        {
            
        }

        // TODO: Datastore failed to reply a Am Alive message
        internal static void onTimeFail(object source, ElapsedEventArgs e)
        {

        }

        internal static void timer()
        {
            // Create a timer with a ten second interval.
            TIMER = new Timer(15000);

            // Hook up the event handler for the Elapsed event.
            TIMER.Elapsed += new ElapsedEventHandler(onTimeFail);

            // Only raise the event the first time Interval elapses.
            TIMER.AutoReset = false;
            TIMER.Enabled = true;
        }


        internal static IDictionary<int, string> getAvailableServers()
        {
            return availableServers;
        }
    }
}
