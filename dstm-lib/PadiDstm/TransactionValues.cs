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
        public void sendUpdatedVal(int uid, int val)
        {
            Console.WriteLine("**TransactionValues called with uid: " + uid + " val: " + val);
            PadiDstm.Transaction.AddValue(uid, val);
            // access the _transaction object on PadiDstm
            // get the PadInt with uid
            // update it's value
        }
    }
}
