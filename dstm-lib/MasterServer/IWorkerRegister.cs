using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IWorkerRegister
    {
        bool registerWorker(string url);
        bool createPadIntMaster(int uid, string client_url);
    }
}
