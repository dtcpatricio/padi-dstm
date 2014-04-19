using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    interface IPadiDstm
    {
        bool Init();
        bool TxBegin();
        bool TxCommit();
        bool TxAbort();

        PadInt CreatePadInt(int uid);
        PadInt AccessPadInt(int uid);
    }
}
