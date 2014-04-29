using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IDatastoreOps
    {
        bool accessPadInt(int uid);
        bool createPadInt(int uid);
        bool commit(int txID, List<string> participants);
    }
}
