using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IRemotePadInt
    {
        int Read(int uid, int txID, string clientURL);
        void Write(int uid, int txID, int newVal, string clientURL);
    }
}
