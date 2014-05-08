using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    internal static class Replica
    {

        internal static void ChangeToReplica(Dictionary<int, string> availableServers)
        {
            //TODO: Change execution mode to replica mode
            
            //TODO: Notify all available servers that i am the replica
            NotifyAllWorkers(availableServers);
        }

        internal static void NotifyAllWorkers(Dictionary<int, string> availableServers)
        {
            foreach (int id in availableServers.Keys)
            {
                //Notify worker that i am the replica
            }
        }

        // Update function called by the workers, receives list of server objects 
        internal static void update(List<ServerObject> updatedList)
        {

        }
    }
}
