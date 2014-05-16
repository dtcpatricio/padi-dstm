using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Threading;


namespace Datastore
{
    class RemotePadInt : MarshalByRefObject, IRemotePadInt
    {

        public int Read(int uid, int txID, string clientURL)
        {
            while (Datastore.STATE.Equals(State.FREEZE))
            {
                Console.WriteLine("SERVER FREEZED!");
                Datastore.Freeze();
            }

            if (Datastore.STATE.Equals(State.FAILED)) {

                //throw new TxException("Failed Server", this);
                return Int32.MinValue;
            }


            int val = Datastore.regTentativeRead(uid, txID, clientURL);
            return val;
        }

        public void Write(int uid, int txID, int newVal, string clientURL)
        {
            if (Datastore.STATE.Equals(State.FAILED)) { throw new ServerFailedException(); }

            bool success = Datastore.regTentativeWrite(uid, newVal, txID, clientURL);

            if (!success)
            {
                // TODO: Put breakpoint inspect modified list
                Console.WriteLine("ABORTED TRANSACTION=" + txID);
                IEndTransaction client = (IEndTransaction)Activator.GetObject(
                    typeof(IEndTransaction), clientURL + "EndTransaction");
                client.abort();
            }
            return;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
