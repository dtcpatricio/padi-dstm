using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.LibraryMaster
{
    public interface ILibraryComm
    {
        int getTxID();
        IDictionary<int, string> updateCache();
    }
}
