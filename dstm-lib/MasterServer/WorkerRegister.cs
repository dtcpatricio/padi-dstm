using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace MasterServer
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
            if (wm.addAvailableWorker(url))
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

        public void printAvailableWorkers()
        {
            Console.WriteLine("Available workers: ");
            for (int i = 0; i < wm.getWorkers().Count; i++)
            {
                Console.WriteLine("\t" + wm.getWorker(i));
            }
        }

        public bool setFail(string url)
        {
            IRemoteOperations worker = (IRemoteOperations)Activator.GetObject(typeof(IRemoteOperations), url);
            worker.Fail();
            return wm.fail(url);
        }

        public bool setFreeze(string url)
        {
            IRemoteOperations worker = (IRemoteOperations)Activator.GetObject(typeof(IRemoteOperations), url);
            worker.Freeze();
            return wm.freeze(url);
        }

        public bool setRecover(string url)
        {
            IRemoteOperations worker = (IRemoteOperations)Activator.GetObject(typeof(IRemoteOperations), url);
            worker.Recover();
            return wm.recover(url);
        }
    }
}
