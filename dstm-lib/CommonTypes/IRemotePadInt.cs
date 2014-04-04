using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IRemotePadInt
    {
        void Read(int uid, string clientURL);
        void Write(int uid, int newVal, string clientURL);
    }
}
