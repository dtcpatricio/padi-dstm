using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;


namespace Datastore
{
    // TODO: Change name
    class RemotePadInt : MarshalByRefObject, IRemotePadInt, IRemoteOperations
    {
        private WorkerState _state;

        PadIntStorage storage;

        public RemotePadInt()
        {
            _state = WorkerState.ACTIVE;
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

            int val = storage.getValue(uid); // palceholder. should be: database.get(uid).value
            ITransactionValues client = (ITransactionValues)Activator.GetObject(typeof(ITransactionValues), clientURL);
            client.sendUpdatedVal(uid, val);
        }

        public void Write(int uid, int newVal, string clientURL)
        {
            // TODO: Register tentative write HERE

            // should be: database.get(uid) = newVal
        }

        public void Fail()
        {
            // TODO: make the server stop responding to external calls
            Console.WriteLine("I'm out of service");
            _state = WorkerState.FAILED;
        }

        public void Freeze()
        {
            // TODO: make the server stop responding to external calls,
            // but maintaining all calls for later reply
            Console.WriteLine("I'm freezed");
            _state = WorkerState.FREEZED;
        }

        public void Recover()
        {
            // TODO: make the server accept calls again
            Console.WriteLine("I'm in service");
            _state = WorkerState.ACTIVE;
        }
    }
}
