using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer
{
    internal static class LibraryManager
    {

        private static int nextTxID = 0;

        internal static int getTxID()
        {
            return ++nextTxID;
        }

    }
}
