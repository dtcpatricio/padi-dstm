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
            if (Datastore.STATE.Equals(State.FAILED)) { return false; }

            bool success = Datastore.checkServerObject(uid);
            return success;
        }

        public bool createPadInt(int uid, int txID, string clientURL)
        {
            if (Datastore.STATE.Equals(State.FAILED)) { return false; }

            while (Datastore.STATE.Equals(State.FREEZE)) { Datastore.Freeze(); }

            bool success = Datastore.createServerObject(uid, txID, clientURL);
            return success;
        }

        public bool commit(int txID, List<string> participants)
        {
            if (Datastore.STATE.Equals(State.FAILED)) { return false; }

            bool success = Datastore.Commit(txID, participants);
            return success;
        }

        public void Fail()
        {
            Console.WriteLine("SERVER FAILED!");
            Datastore.STATE = State.FAILED;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
