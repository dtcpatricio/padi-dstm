using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    internal interface ICoordinator
    {
        void sendYes(int txID, string url);
        void sendNo(int txID, string url);
        void haveCommitted(int txID, string url);
    }
}
