using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.DatastoreMaster
{
    public interface IDatastoreComm
    {
        bool registerWorker(string url);
    }
}
