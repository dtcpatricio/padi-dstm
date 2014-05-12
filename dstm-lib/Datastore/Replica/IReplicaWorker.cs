using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    internal interface IReplicaWorker
    {
        //Sends the replica url
        List<ServerObject> fetchData(string url);
    }
}
