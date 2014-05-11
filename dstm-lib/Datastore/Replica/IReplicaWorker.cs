using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    public interface IReplicaWorker
    {
        //Sends the replica url
        List<ServerObject> fetchData(string url);
    }
}
