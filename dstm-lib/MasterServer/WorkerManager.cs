using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer
{
    class WorkerManager
    {
        // Maps uid with worker references
        private IDictionary<int, string> padIntUids;

        // List of available workers
        private List<string> workers;

        // Index to workers refering to the next available worker
        private int nextAvailableWorker;

        public WorkerManager()
        {
            padIntUids = new Dictionary<int, string>();
            workers = new List<string>();
            nextAvailableWorker = -1;
        }

        // Assign to server uid in a round robin fashion
        public void assignServer(int uid)
        {
            addUid(uid, getNextAvailableWorker());
        }

        public void addUid(int uid, string url)
        {
            padIntUids.Add(uid, url);
        }

        public string getUidUrl(int uid)
        {
            return padIntUids[uid];
        }

        // Returns true if worker was added successfully, false otherwise
        public bool registerWorker(string url)
        {
            if (workers.Contains(url))
            {
                return false;
            }
            workers.Add(url);
            return true;
        }

        // returns url of the next available worker, following a round robin fashion
        public string getNextAvailableWorker()
        {
            if (nextAvailableWorker == workers.Count)
            {
                nextAvailableWorker = 0;
            }
            else
            {
                nextAvailableWorker++;
            }

            return workers[nextAvailableWorker];
        }

        public List<string> getWorkers()
        {
            return workers;
        }

        public string getWorker(int index)
        {
            return workers[index];
        }
    }
}
