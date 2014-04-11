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
        bool setFail(string url);
        bool setFreeze(string url);
        bool setRecover(string url);
        int getNextTransactionId();
        bool createPadIntMaster(int tid, int uid, string client_url);
    }
}
