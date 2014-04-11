using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer
{
    public interface IWorkerRegister
    {
        bool registerWorker(string url);
    }
}
