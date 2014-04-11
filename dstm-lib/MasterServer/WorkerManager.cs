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
        private List<string> available_workers;

        // List of failed workers
        private List<string> failed_workers;

        // Index to workers refering to the next available worker
        private int nextAvailableWorker;

        // Maps Id of transactions with the array of Transaction UID's 
        // in order for the commit operation be able to identify all objects
        // belonging to the transaction
        private IDictionary<int, ArrayList> transactions;

        public WorkerManager()
        {
            padIntUids = new Dictionary<int, string>();
            available_workers = new List<string>();
            failed_workers = new List<string>();
            transactions = new Dictionary<int, ArrayList>();
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
        public bool addAvailableWorker(string url)
        {
            if (available_workers.Contains(url))
            {
                return false;
            }
            available_workers.Add(url);
            return true;
        }

        // returns url of the next available worker, following a round robin fashion
        public string getNextAvailableWorker()
        {
            if (nextAvailableWorker == available_workers.Count)
            {
                nextAvailableWorker = 0;
            }
            else
            {
                nextAvailableWorker++;
            }

            return available_workers[nextAvailableWorker];
        }

        public List<string> getWorkers()
        {
            return available_workers;
        }

        public string getWorker(int index)
        {
            return available_workers[index];
        }

        public bool availableWorkerExists(string url)
        {
            if (available_workers.Contains(url))
            {
                return true;
            }
            return false;
        }

        public bool failedWorkerExists(string url)
        {
            if (failed_workers.Contains(url))
            {
                return true;
            }
            return false;
        }

        public void removeAvailableWorker(string url) {
            available_workers.Remove(url);
        }

        public void removeFailedWorker(string url)
        {
            failed_workers.Remove(url);
        }

        public void addFailedWorker(string url) {
            failed_workers.Add(url);
        }

        public bool fail(string url)
        {
            if (availableWorkerExists(url))
            {
                removeAvailableWorker(url);
                addFailedWorker(url);
                Console.WriteLine("URL " + url + " out of service.");
                return true;
            }
            Console.WriteLine("URL " + url + " does not exist.");
            return false;
        }

        public bool freeze(string url)
        {
            if (availableWorkerExists(url))
            {
                removeAvailableWorker(url);
                addFailedWorker(url);   // inform is freezed
                Console.WriteLine("URL " + url + " freezed.");
                return true;
            }
            Console.WriteLine("URL " + url + " does not exist.");
            return false;
        }

        public bool recover(string url)
        {
            if (failedWorkerExists(url))
            {
                removeFailedWorker(url);
                addAvailableWorker(url);
                Console.WriteLine("URL " + url + " has recovered.");
                return true;
            }
            Console.WriteLine("URL " + url + " does not exist.");
            return false;
        }

        // Assign a new transaction id
        public int getNextTransactionId()
        {
            int new_id;
            lock (this)
            {
                new_id = transactions.Count;
                ArrayList newArray = new ArrayList();
                transactions.Add(new_id, newArray);
            }
            return new_id;
        }

        public void addTransactionUid(int txid, int uid)
        {
            ArrayList uidArray = transactions[txid];
            uidArray.Add(uid);
        }

        // Assign to server uid in a round robin fashion
        public bool assignWorker(int uid)
        {
            //addUid(uid, getNextAvailableWorker());
            if (!padIntUids.ContainsKey(uid))
            {
                string availableWorker = getNextAvailableWorker();
                Console.WriteLine("UID: " + uid + " AvailableWorker : " + availableWorker);
                padIntUids.Add(uid, availableWorker);
                return true;
            }
            return false;
        }

        public string getWorkerUrl(int uid)
        {
            return padIntUids[uid];
        }
    }
}
