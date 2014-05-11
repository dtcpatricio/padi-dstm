using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace Datastore
{
    internal static class Replica
    {
        // Worker's replica url
        private static string _replicaURL;

        // List of stored servers objects naturally orded from available servers in Master
        private static Dictionary<int, List<ServerObject>> storedServerObjects = new Dictionary<int, List<ServerObject>>();


        internal static string REPLICAURL
        {
            get { return _replicaURL; }
            set { _replicaURL = value; }
        }

        
        internal static void ChangeToReplica(Dictionary<int, string> availableServers)
        {
           //TODO: Change execution mode to replica mode
            Datastore.startReplicaMode(availableServers);         
        }

        //TODO: Notify all available servers that i am the replica, the datastore url
        internal static void NotifyAllWorkers(Dictionary<int, string> availableServers)
        {
            foreach (int id in availableServers.Keys)
            {
                //Notify worker that i am the replica
                IReplicaWorker worker = (IReplicaWorker)Activator.GetObject(
                    typeof(IReplicaWorker), availableServers[id] + "ReplicaWorker");
                worker.setReplica(Datastore.SERVERURL);
            }
        }

        // Update function called by the workers, receives list of server objects 
        internal static void update(List<ServerObject> updatedList)
        {

        }
    }
}
