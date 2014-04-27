using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.LibraryDatastore;


namespace Datastore
{
    class RemotePadInt : MarshalByRefObject, IRemotePadInt
    {

        public int Read(int uid, int txID, string clientURL)
        {
            
            int val = Datastore.regTentativeRead(uid, txID, clientURL);

            return val;
        }

        public void Write(int uid, int txID, int newVal, string clientURL)
        {
            
            bool success = Datastore.regTentativeWrite(uid, newVal, txID, clientURL);

            if (!success)
            {
                IEndTransaction client = (IEndTransaction)Activator.GetObject(typeof(IEndTransaction), clientURL);
                client.abort();
            }
        }
    }
}
