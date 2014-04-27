using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    class Coordinator : MarshalByRefObject, ICoordinator
    {
        public void sendYes(int txID, string url)
        {
            Datastore.TRANSACTIONMANAGER.voteYes(url);
        }
        
        public void sendNo(int txID, string url)
        {
            Datastore.TRANSACTIONMANAGER.voteNo(url);
        }

        public void haveCommitted(int txID, string url)
        {

        }
    }
}
