using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    internal static class Datastore
    {

        // TODO: refactor to use generic list classes

        private static List<TentativeTx> tentativeTransactions = new List<TentativeTx>();

        /**
         * Registers a tentative READ
         * @return VOID
         **/
        internal static void regTentativeRead(int uid, string clientURL)
        {

        }

        /**
         * Registers a tentative Write
         * @return TRUE if the value could be written
         * @return FALSE if the write is in conflict, cascading into an abort Tx
         **/
        internal static bool regTentativeWrite(int uid, int newVal, string clientURL)
        {

            return true;
        }
    }
}
