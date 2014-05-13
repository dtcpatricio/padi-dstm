using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace Datastore
{
    class DatastoreOps : MarshalByRefObject, IDatastoreOps
    {
        public bool accessPadInt(int uid)
        {
            bool success = Datastore.checkServerObject(uid);
            return success;
        }

        public bool createPadInt(int uid, int txID, string clientURL)
        {
            bool success = Datastore.createServerObject(uid, txID, clientURL);
            return success;
        }

        public bool commit(int txID, List<string> participants)
        {
            bool success = Datastore.Commit(txID, participants);
            return success;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
