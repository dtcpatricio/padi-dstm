using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.LibraryMaster;

namespace PADI_DSTM
{
    internal class ServersCache
    {
        private IDictionary<int, string> _availableServers;

        internal IDictionary<int, string> AvailableServers { get { return _availableServers; } }

        internal ServersCache()
        {        }

        internal void updateCache()
        {
            ILibraryComm master = (ILibraryComm)Activator.GetObject(
                typeof(ILibraryComm),
                PadiDstm.Master_Url + "LibraryComm");

            _availableServers = master.updateCache();
        }

    }
}
