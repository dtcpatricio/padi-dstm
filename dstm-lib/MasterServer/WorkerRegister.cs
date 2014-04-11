using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
