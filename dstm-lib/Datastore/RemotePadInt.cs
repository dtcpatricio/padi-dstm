using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;


namespace Datastore
{
    class RemotePadInt : MarshalByRefObject, IRemotePadInt
    {

        public void Read(int uid, string clientURL)
        {
            // TODO: Register tentative read HERE

            int val = 1; // palceholder. should be: database.get(uid).value

            ITransactionValues client = (ITransactionValues)Activator.GetObject(typeof(ITransactionValues), clientURL);
            client.sendUpdatedVal(val, uid);
        }

        public void Write(int uid, int newVal, string clientURL)
        {
            // TODO: Register tentative write HERE

            // should be: database.get(uid) = newVal
        }
    }
}
