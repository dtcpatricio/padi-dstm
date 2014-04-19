using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.LibraryDatastore;

namespace PADI_DSTM
{
    class EndTransaction : MarshalByRefObject, IEndTransaction
    {
        public void abort()
        {

        }

        public void commit()
        {

        }
    }
}
