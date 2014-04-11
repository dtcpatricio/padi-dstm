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
        PadIntStorage storage;

        public RemotePadInt()
        {
            storage = new PadIntStorage();
        }

        public string getWorkerUrl()
        {
            return Datastore.Worker._worker_url;
        }

        // -1 means that we have createad a PadInt uninitialized
        public void createPadIntWorker(int uid, string client_url)
        {
            storage.addPadInt(uid, -1);
            ITransactionValues remote = (ITransactionValues)Activator.GetObject(
                typeof(ITransactionValues),
                client_url);

            Console.WriteLine("RemotePadInt.createPadInt uid: " + uid + " " + client_url);
            remote.sendUpdatedVal(uid, storage.getValue(uid));
        }

        public void Read(int uid, string clientURL)
        {
            // TODO: Register tentative read HERE

            int val = 1; // palceholder. should be: database.get(uid).value

            ITransactionValues client = (ITransactionValues)Activator.GetObject(typeof(ITransactionValues), clientURL);
            client.sendUpdatedVal(uid, val);
        }

        public void Write(int uid, int newVal, string clientURL)
        {
            // TODO: Register tentative write HERE

            // should be: database.get(uid) = newVal
        }
    }
}
