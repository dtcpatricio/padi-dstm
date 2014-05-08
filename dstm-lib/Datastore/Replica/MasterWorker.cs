using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace Datastore
{
    class MasterWorker : MarshalByRefObject, IMasterWorker
    {
        public void setAsReplica(Dictionary<int, string> availableServers)
        {
            Replica.ChangeToReplica(availableServers);
        }
    }
}
