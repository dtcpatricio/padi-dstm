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

        internal static IDictionary<int, string> getAvailableServers()
        {
            return availableServers;
        }

        // Returns the id predecessor in replication chain
        internal static int getAvailableID(string url)
        {
            int id = availableServers.FirstOrDefault(x => x.Value.Equals(url)).Key;
            return id;
        }

        internal static int getFreezeID(string url)
        {
            int id = freezedServers.FirstOrDefault(x => x.Value.Equals(url)).Key;
            return id;

        }
        // Returns the id sucessor in replication chain
        internal static string getWorkerSucessor(int id)
        {
            if (id == availableServers.Count - 1)
                return availableServers[0];
            return availableServers[id + 1];
        }

        // Returns the id predecessor in replication chain
        internal static string getWorkerPredecessor(int id)
        {
            if (id == 0)
                return availableServers[availableServers.Count - 1];
            return availableServers[id - 1];
        }

        internal static string getFailedSucessor(int failed_id)
        {
            // Return the sucessor of the failed server
            if (availableServers.ContainsKey(failed_id + 1))
                return availableServers[failed_id + 1];

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
            if (freezedServers.ContainsValue(url))
                return true;

            return false;
        }

        // Master tells the worker to freeze itself
        internal static bool freeze(string url)
        {
            if (isFailedServer(url) || isFreezedServer(url)) return false;

            IMasterWorker worker = (IMasterWorker)Activator.GetObject(
                typeof(IMasterWorker), url + "MasterWorker");

            int id = getAvailableID(url);

            freezedServers.Add(id, url);
            worker.freeze();

            return true;
        }

        internal static bool recover(string url)
        {
            if (isFailedServer(url))
                return true;

            if (isFreezedServer(url))
            {
                int freeze_id = getFreezeID(url);
                freezedServers.Remove(freeze_id);
                availableServers[freeze_id] = url;
                IMasterWorker datastore = (IMasterWorker)Activator.GetObject(typeof(IMasterWorker), url + "MasterWorker");
                datastore.recover();
                return true;
            }
            return false;
        }

        // Returns the total number of servers incl94e0uding the replica and failed servers
        internal static int totalServers()
        {
            int total = 0;
            return availableServers.Count + failedServers.Count + total;
        }


        /**
         * Add a new server into the system
         * A freezed server is also an "available" server,
         * hence we only need to test if the url is already in failed or available
         */
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

        // Set the sucessor replica of the worker
        internal static void SetReplica(string worker_url, string sucessor, string predecessor)
        {
            IMasterWorker remote = (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                worker_url + "MasterWorker");
            remote.setReplica(sucessor, predecessor);
        }

        // Sets the detected url to failed
        internal static string setFailedServer(string url)
        {
            // simple sanity check
            if (!failedServers.ContainsValue(url))
            {
                // if the server was available do the following
                if (availableServers.ContainsValue(url))
                {
                    lock (availableServers)
                    {
                        int id = getAvailableID(url);
                        int sucessorID = id + 1;
                        if (sucessorID >= availableServers.Count)
                            sucessorID = 0;
                        int predecessorID = id - 1;
                        if (predecessorID < 0)
                            predecessorID = availableServers.Count;

                        string predecessorURL = availableServers[predecessorID];
                        string sucessorURL = availableServers[sucessorID];
                        string failedURL = availableServers[id];

                        IMasterWorker sucessor =
                                        (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                                          sucessorURL + "MasterWorker");
                        IMasterWorker predecessor =
                                        (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                                          predecessorURL + "MasterWorker");
                        IMasterWorker failed =
                                        (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                                          failedURL + "MasterWorker");
                        availableServers[id] = sucessorURL; // same as availableServers[successorID]
                        sucessor.setPredecessor(predecessorURL);
                        predecessor.setSucessor(sucessorURL);
                        sucessor.substituteFailedServer();
                        failed.fail();
                        failedServers.Add(id, url);
                        return sucessorURL;
                    }
                }
                else if (freezedServers.ContainsValue(url))
                {
                    lock (freezedServers)
                    {
                        int id = getFreezeID(url);
                        int sucessorID = id + 1;
                        if (sucessorID >= freezedServers.Count)
                            sucessorID = 0;
                        int predecessorID = id - 1;
                        if (predecessorID < 0)
                            predecessorID = freezedServers.Count;

                        string predecessorURL = freezedServers[predecessorID];
                        string sucessorURL = freezedServers[sucessorID];
                        string failedURL = freezedServers[id];

                        IMasterWorker sucessor =
                                        (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                                          sucessorURL + "MasterWorker");
                        IMasterWorker predecessor =
                                        (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                                          predecessorURL + "MasterWorker");
                        IMasterWorker failed =
                                        (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                                          failedURL + "MasterWorker");
                        // the server was both in available and freezed servers
                        availableServers[id] = sucessorURL;
                        sucessor.setPredecessor(predecessorURL);
                        predecessor.setSucessor(sucessorURL);
                        sucessor.substituteFailedServer();
                        failed.fail();
                        freezedServers.Remove(id);
                        failedServers.Add(id, url);
                        return sucessorURL;
                    }
                }

            }
            return null;
        }
    }
}
