using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.DatastoreMaster
{
    interface IReplicationComm
    {
        //Warning: What if the message does not arrive
        void setAsBackup(Dictionary<int, string> availableServers);
    }
}
