using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CommonTypes
{
    class WorkerRegister : MarshalByRefObject, IWorkerRegister
    {
        WorkerManager wm;

        public WorkerRegister()
        {
            wm = new WorkerManager();
        }

        // Returns true if worker was added successfully, false otherwise
        public bool registerWorker(string url)
        {
            if (wm.registerWorker(url))
            {
                Console.WriteLine("Worker in url " + url + " added.");
                printAvailableWorkers();
                return true;
            }
            else
            {
                Console.WriteLine("Worker already exists");
                return false;
            }
        }

        // It should return an exception instead of carrying around bool values
        // Missing : add the uid object to the given transaction
        public bool createPadIntMaster(int txid, int uid, string client_url)
        {
            Console.WriteLine("Arrived at master with uid: " + uid);
            string worker_url;
            bool isAssigned = wm.assignWorker(uid);
            Console.WriteLine(isAssigned);
            if (isAssigned)
            {
                worker_url = wm.getWorkerUrl(uid);
                
                IRemotePadInt remote = (IRemotePadInt)Activator.GetObject(
                typeof(IRemotePadInt),
                worker_url);

                Console.WriteLine("createPadIntMaster txid: " + txid);
                wm.addTransactionUid(txid, uid);

                Console.WriteLine("Calling Worker Url: " + worker_url);
                remote.createPadIntWorker(uid, client_url);
                return true;
            } 
            return false;
        }

        public int getNextTransactionId()
        {
            return wm.getNextTransactionId();
        }

        public void printAvailableWorkers()
        {
            Console.WriteLine("Available workers: ");
            for (int i = 0; i < wm.getWorkers().Count; i++)
            {
                Console.WriteLine("\t" + wm.getWorker(i));
            }
        }
    }
}
