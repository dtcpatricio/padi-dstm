using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Net.Sockets;

namespace Datastore
{
    internal static class Replica
    {
        // List of stored servers objects naturally orded from available servers in Master
        private static List<ServerObject> worker_serverObjects = new List<ServerObject>();

        private static string _sucessor;
        private static string _predecessor;

        internal static string SUCESSOR
        {
            get { return _sucessor; }
            set { _sucessor = value; }
        }

        internal static string PREDECESSOR
        {
            get { return _predecessor; }
            set { _predecessor = value; }
        }

        internal static List<ServerObject> WORKERSERVEROBJECTS
        {
            get { return worker_serverObjects; }
            set { worker_serverObjects = value; }
        }


        // Update function called by the workers, receives list of server objects 
        // from a given server. If worker_url does not exist in worker_serverObjects
        // it means that it is a new server

        //Send updated to sucessor, if sucessor died return false -> transaction abort
        //call master to stabalize the system
        internal static UpdateState updateSucessor(List<ServerObject> writtenObjects)
        {
            try
            {
                IWorkerReplica sucessor = (IWorkerReplica)Activator.GetObject(typeof(IWorkerReplica), Replica.SUCESSOR + "WorkerReplica");
                sucessor.update(writtenObjects);
                return UpdateState.COMMIT;
            }
            catch (SocketException)
            {
               // manageFailedServer(Replica.SUCESSOR);
                //updateSucessor(writtenObjects);
                return UpdateState.ABORT;
            }
            catch (System.IO.IOException)
            {
                //manageFailedServer(Replica.SUCESSOR);
                //updateSucessor(writtenObjects);
                return UpdateState.ABORT;
            }
        }

        // Manage the failure, the Master alters the sucessors of this class
        internal static void manageFailedServer(string failed_url)
        {
            Console.WriteLine("INSIDE CATCH IN DATASTORE!!!");

            string sucessor_url;

            ILibraryComm master = (ILibraryComm)Activator.GetObject(
            typeof(ILibraryComm), Datastore.MASTER + "LibraryComm");
            sucessor_url = master.setFailedServer(failed_url);

            // Server failed
            if (sucessor_url == null)
                Console.WriteLine("ERROR: THERE ARE NO SERVERS THAT HAVE THE REQUESTED PADINT");
        }

        
        // Receive update from another datasore

        /**
         * Method to backup the written objects on the transaction
         * Only called by another datastore
         * @path Participant->doCommit->Replica.updateSucessor->update
         */
        internal static void update(List<ServerObject> updatedList)
        {
            lock (worker_serverObjects)
            {
                // debug info
                foreach (ServerObject so in updatedList)
                {
                    Console.WriteLine("\t" + "Current Server Updating UID=" + so.UID + " VALUE=" + so.VALUE);
                }

                List<ServerObject> oldList = worker_serverObjects;

                int j = 0;
                bool updated = false;

                foreach (ServerObject updatedSO in updatedList)
                {
                    foreach (ServerObject oldSO in oldList)
                    {
                        if (oldSO.UID == updatedSO.UID)
                        {
                            worker_serverObjects[j] = updatedSO;
                            j = 0;
                            updated = true;
                            break;
                        }
                        j++;
                    }
                    if (updated == false)
                    {
                        worker_serverObjects.Add(updatedSO);
                        updated = false;
                    }
                    j = 0;
                }
            }
        }

        //Notify predecessor and sucessor 
        internal static void setReplica(string sucessor, string predecessor)
        {
            SUCESSOR = sucessor;
            PREDECESSOR = predecessor;

            // Notify sucessor (sucessor, myURL -> predecessor)
            if (!sucessor.Equals(Datastore.SERVERURL) && (!predecessor.Equals(Datastore.SERVERURL)))
            {
                notifySucessor(sucessor, Datastore.SERVERURL);
                notifyPredecessor(predecessor, Datastore.SERVERURL);
            }
        }

        internal static void notifySucessor(string sucessor, string myURL)
        {
            IWorkerReplica replica = (IWorkerReplica)Activator.GetObject(
                    typeof(IWorkerReplica), sucessor + "WorkerReplica");
            replica.setPredecessor(myURL);
        }

        internal static void notifyPredecessor(string predecessor, string myURL)
        {
            IWorkerReplica replica = (IWorkerReplica)Activator.GetObject(
                    typeof(IWorkerReplica), predecessor + "WorkerReplica");

            worker_serverObjects = replica.setSucessor(myURL);
        }

    }
}
