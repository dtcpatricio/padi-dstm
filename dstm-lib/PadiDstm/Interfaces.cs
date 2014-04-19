using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{

    interface IServerCtrlOps
    {
        bool Fail(string URL);
        bool Freeze(string URL);
        bool Recover(string URL);
    }

    interface IMiscOps
    {
        bool Status();
    }
}
