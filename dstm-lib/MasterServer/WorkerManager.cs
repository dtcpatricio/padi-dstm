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

        // Returns the id predecessor in replication chain
        internal static int getFailedID(string url)
        {
            int id = failedServers.FirstOrDefault(x => x.Value.Equals(url)).Key;
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

        // Master tells the worker to fail itself and stabilizes the system
        // TODO: Stabilize the system
        internal static bool fail(string url)
        {
            if (isFailedServer(url)) { return false; }

            int id = getAvailableID(url);

            if (isFreezedServer(url)) { freezedServers.Remove(id); }
            failedServers.Add(id, url);

            string failed_sucessorURL = getWorkerSucessor(id);
            string failed_predecessorURL = getWorkerPredecessor(id);

            IMasterWorker failed_sucessor = (IMasterWorker)Activator.GetObject(
                typeof(IMasterWorker), failed_sucessorURL + "MasterWorker");
            IMasterWorker failed_predecessor = (IMasterWorker)Activator.GetObject(
                typeof(IMasterWorker), failed_predecessorURL + "MasterWorker");

            // Na posicao original dos available trocar o url pelo sucessor
            availableServers[id] = failed_sucessorURL;

            //Set sucessor of failed_predecessor to failed_sucessor -> Problema de concorrencia entre estas 2 operções?
            failed_predecessor.setSucessor(failed_sucessorURL);

            //Tell the sucessor to substitute the failed server
            failed_sucessor.substituteFailedServer();

            //Tell the sucessor to fetch the original data from the failed_predecessor and put it in his the list of updates
            failed_sucessor.fetch_data(failed_predecessorURL);

            //Tell the failed_predecessor to fetch the data of failed_sucessor
            // WARNING: Verificar o caso me que o sucessor é ele próprio, será que consegue enviar a msg?
            //            failed_predecessor.fetch_data(failed_sucessorURL);

            // Last thing to do is change the state, so that the library can continue working 

            IMasterWorker datastore = (IMasterWorker)Activator.GetObject(
                typeof(IMasterWorker), url + "MasterWorker");

            datastore.fail();

            return true;
        }

        internal static bool recover(string url)
        {
            if (isFailedServer(url))
            {
                int failed_id = getFailedID(url);
                failedServers.Remove(failed_id);

                // Sets the original URL
                availableServers[failed_id] = url;

                string failed_sucessorURL = getWorkerSucessor(failed_id);
                string failed_predecessorURL = getWorkerPredecessor(failed_id);

                IMasterWorker failed_predecessor = (IMasterWorker)Activator.GetObject(
                    typeof(IMasterWorker), failed_predecessorURL + "MasterWorker");

                IMasterWorker datastore = (IMasterWorker)Activator.GetObject(
                typeof(IMasterWorker), url + "MasterWorker");

                // Set sucessor of the failed predecessor to the recovered server
                failed_predecessor.setSucessor(url);

                // Fetch data from from failed predecessor to put in his list of updates
                datastore.fetch_data(failed_predecessorURL);

                // Fetch recover data to recovered server to fetch the primary data of his sucessor
                //  also sets his sucessor
                datastore.fetch_recover_data(failed_sucessorURL);

                return true;
            }
            if (isFreezedServer(url))
            {
                int freeze_id = getFreezeID(url);
                freezedServers.Remove(freeze_id);
                availableServers[freeze_id] = url;
                IMasterWorker worker = (IMasterWorker)Activator.GetObject(typeof(IMasterWorker), url + "MasterWorker");
                worker.recover();
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

        internal static void status()
        {
            foreach (String server_url in availableServers.Values)
            {
                if (!isFailedServer(server_url) || !isFreezedServer(server_url))
                {
                    IMasterWorker worker = (IMasterWorker)Activator.GetObject(typeof(IMasterWorker),
                        server_url + "MasterWorker");
                    worker.status();
                }
            }
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
