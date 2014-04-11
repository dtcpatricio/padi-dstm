using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    class WorkerManager
    {
        // Maps uid with worker references
        private IDictionary<int, string> padIntUids;

        // Maps Id of transactions with the array of Transaction UID's 
        // in order for the commit operation be able to identify all objects
        // belonging to the transaction
        private IDictionary<int, ArrayList> transactions;
        
        // List of available workers
        private List<string> workers;

        // Index to workers refering to the next available worker
        private int nextAvailableWorker;

        public WorkerManager()
        {
            padIntUids = new Dictionary<int, string>();
            transactions = new Dictionary<int, ArrayList>();
            workers = new List<string>();
            nextAvailableWorker = -1;
        }

        public IDictionary<int, ArrayList> getTransactions()
        {
            return transactions;
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

           // return workers[nextAvailableWorker];
            return workers[0]; // placeholder
        }

        public List<string> getWorkers()
        {
            return workers;
        }

        public string getWorker(int index)
        {
            return workers[index];
        }

        public string getWorkerUrl(int uid)
        {
            return padIntUids[uid];
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

    }
}
