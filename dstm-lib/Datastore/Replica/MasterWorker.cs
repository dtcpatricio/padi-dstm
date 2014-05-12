using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace Datastore
{
    class MasterWorker : MarshalByRefObject, IMasterWorker
    {
       /* public void setAsReplica(Dictionary<int, string> availableServers)
        {
            Replica.ChangeToReplica(availableServers);
        }

        // The master ID of the worker server to replace
        public void setWorker(int id)
        {
            Console.WriteLine("NOW IM A WORKER WITH ID=" + id);
            Replica.changeToWorker(id);
        }
        */
        // The master ID of the worker server to replace
        public void setReplica(string sucessor, string predecessor)
        {
            Console.WriteLine("NOW MY SUCESSOR IS = " + sucessor + " PREDECESSOR IS = " + predecessor);
            Replica.setReplica(sucessor, predecessor);
        }
    }
}
