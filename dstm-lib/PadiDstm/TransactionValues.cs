using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace Padi_dstm
{
    class TransactionValues : MarshalByRefObject, ITransactionValues
    {
        public void sendUpdatedVal(int val, int uid)
        {
            // access the _transaction object on PadiDstm
            // get the PadInt with uid
            // update it's value
        }
    }
}
