using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface ILibraryComm
    {
        int getTxID();
        IDictionary<int, string> updateCache();
        
        // returns the url of the sucessor that has the UID objects
        string setFailedServer(string failed_url);

        bool freeze(string server_url);
    }
}
