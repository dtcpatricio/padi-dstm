using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Padi_dstm
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
