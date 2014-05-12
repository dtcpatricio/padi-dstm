using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IMasterWorker
    {
        void setAsReplica(Dictionary<int, string> availableServers);
        void setReplica(string url);
        void setWorker(int id);
    }
}
