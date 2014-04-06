using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace padi_dstm_library
{
    public interface Library
    {
        PadInt CreatePadInt(int uid);
        PadInt AccessPadInt(int uid);
       // int read();
    }
}
