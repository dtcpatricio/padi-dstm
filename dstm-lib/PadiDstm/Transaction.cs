using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using CommonTypes;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Timers;


namespace PADI_DSTM
{
    public class Transaction
    {
        private int _txID;

        // removed private set from State because of padi-dstm
        internal TransactionState State { get; set; }

        private TcpChannel channel;

        // maps padint uids with the values
        private Dictionary<int, int> values = new Dictionary<int, int>();

        // list of the servers accessed within the transaction
        private List<string> accessedServers = new List<string>();

        internal int TXID { get { return _txID; } }

        // UID has a temporary lock
        internal Dictionary<int, bool> freeze_lock = new Dictionary<int, bool>();

        static bool failed_lock = false;
        static bool write_lock = false;

        private const int TIMETOFAILURE = 15000;

        private System.Timers.Timer _timer;

        public delegate int ReadAsyncDelegate(int uid, int txID, string url);
        public delegate void WriteAsyncDelegate(int uid, int txID, int val, string url);

        // Timer for remote operations, detect freeze, recover and server failures


        // Returns the dictionary of all the values (uid, value) kept in the current transaction
        internal Dictionary<int, int> GetValues { get { return this.values; } }

        // returns the list of the accessed servers throughout the transaction
        internal List<string> ACCESSEDSERVERS { get { return accessedServers; } }

        internal Transaction(TcpChannel channel)
        {
            this.channel = channel;
            // aquire an unique transaction ID
            ILibraryComm master = (ILibraryComm)Activator.GetObject(
                typeof(ILibraryComm),
                PadiDstm.Master_Url + "LibraryComm");
            _txID = master.getTxID();

            // placeholder debug message
            Console.WriteLine("Started new transaction with id : " + _txID);
            // set the state of the transaction to ACTIVE
            State = TransactionState.ACTIVE;
        }

        // TODO: Detect failure if worker fails to reply
        internal void timerAlive(string server_url)
        {
            // Create a timer with a TIMETOFAILURE interval.
            _timer = new System.Timers.Timer(TIMETOFAILURE);

            // Hook up the event handler for the Elapsed event.
            _timer.Elapsed += (source, e) => onTimeFail(source, e, server_url);

            // Only raise the event the first time Interval elapses.
            _timer.AutoReset = false;
            _timer.Enabled = true;
        }


        // Tell Master to kill it 
        internal void onTimeFail(object source, ElapsedEventArgs e, string server_url)
        {
            Console.WriteLine("Server " + server_url + " Died!");
            _timer.Enabled = false;
            _timer.AutoReset = false;

            ILibraryComm master = (ILibraryComm)Activator.GetObject(
                        typeof(ILibraryComm), PadiDstm.Master_Url + "LibraryComm");

            failed_lock = true;

            // master.setFailedServer(server_url);
            master.fail(server_url);

            State = TransactionState.ABORTED;

            //Update to the new list of servers
            // WARNING: It seems now it is not necessary
            PadiDstm.Servers.updateCache();
            failed_lock = false;
        }

        internal void resetTimer()
        {
            _timer.Enabled = false;
            _timer.AutoReset = false;
        }

        // This is the call that the AsyncCallBack delegate will reference.
        public void ReadAsyncCallBack(IAsyncResult ar)
        {
            while (failed_lock == true) { Thread.Sleep(250); }

            lock (this)
            {
                ReadAsyncDelegate del = (ReadAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
                PadInt padInt = (PadInt)ar.AsyncState;

                int value = del.EndInvoke(ar);

                //In case the server never responds -> force kill 
                if (value == Int32.MinValue)
                {

                    // TODO: Kill the server
                    State = TransactionState.ABORTED;

                    /*padInt.URL = PadiDstm.Servers.AvailableServers[PadiDstm.computeDatastore(padInt.UID)];
                    int value2 = reRead(padInt);
                    AddValue((int)padInt.UID, value2);*/
                    failed_lock = false;
                    freeze_lock[(int)padInt.UID] = true;

                    return;
                }
                AddValue((int)padInt.UID, value);

                freeze_lock[(int)padInt.UID] = true;
            }
        }

        // This is the call that the AsyncCallBack delegate will reference.
        public void WriteAsyncCallBack(IAsyncResult ar)
        {
            while (failed_lock == true) { Thread.Sleep(250); }

            failed_lock = false;
            write_lock = false;         
        }


        /*
        internal int reRead(PadInt padInt)
        {
            lock (this)
            {
                string remotePadIntURL = padInt.URL + "RemotePadInt";
                int uid = padInt.UID;


                IRemotePadInt remote = (IRemotePadInt)Activator.GetObject(
                    typeof(IRemotePadInt), remotePadIntURL);

                // call the RemotePadInt to get the value
                int val = remote.Read(uid, TXID, PadiDstm.Client_Url);
                AddValue(uid, val);

                addAccessedServer(padInt.URL);
                return val;
            }
        }
        */

        internal int Read(PadInt padInt)
        {
            if (State.Equals(TransactionState.ABORTED))
            {
                // Some server failed
                Console.WriteLine("Trying to access padint uid=" + padInt.UID + " denied because of server failure");
                return -1;
            }
            string remotePadIntURL = padInt.URL + "RemotePadInt";
            int uid = padInt.UID;

            freeze_lock.Add(uid, false);

            IRemotePadInt remote = (IRemotePadInt)Activator.GetObject(
                typeof(IRemotePadInt), remotePadIntURL);

            ReadAsyncDelegate RemoteDel = new ReadAsyncDelegate(remote.Read);
            AsyncCallback RemoteCallback = new AsyncCallback(ReadAsyncCallBack);

            try
            {
                IAsyncResult RemAr = RemoteDel.BeginInvoke(uid, TXID, PadiDstm.Client_Url, RemoteCallback, padInt);
            }
            catch (TxException t) { Console.WriteLine("TXEXCEPTION" + t.msg); }

            timerAlive(padInt.URL);
            while (!freeze_lock[uid]) { Thread.Sleep(250); }
            resetTimer();
            lock (freeze_lock) { freeze_lock.Remove(uid); }

            if (State.Equals(TransactionState.ABORTED))
            {
                // Some server failed
                Console.WriteLine("Trying to access padint uid=" + padInt.UID + " denied because of server failure");
                return -1;
            }
            return values[uid];
        }

        internal void Write(PadInt padInt, int val)
        {
            if (State.Equals(TransactionState.ABORTED))
            {
                Console.WriteLine("Write padint UID= " + padInt.UID + "denied because of server failer");
                return;
            }

            string remotePadIntURL = padInt.URL + "RemotePadInt";
            int uid = padInt.UID;

            IRemotePadInt remote = (IRemotePadInt)Activator.GetObject(
                typeof(IRemotePadInt), remotePadIntURL);

            write_lock = true;
            WriteAsyncDelegate RemoteDel = new WriteAsyncDelegate(remote.Write);
            AsyncCallback RemoteCallback = new AsyncCallback(WriteAsyncCallBack);
            IAsyncResult RemAr = RemoteDel.BeginInvoke(uid, TXID, val, PadiDstm.Client_Url, RemoteCallback, padInt);

            timerAlive(padInt.URL);
            while (write_lock) { Thread.Sleep(250); }

            resetTimer();
            
            //lock (freeze_lock) { freeze_lock.Remove(uid); }

            if (State.Equals(TransactionState.ABORTED))
            {
                Console.WriteLine("Write padint UID= " + padInt.UID + "denied because of server failer");
                return;
            }

            //  remote.Write(uid, TXID, val, PadiDstm.Client_Url);
            AddValue(uid, val);
            addAccessedServer(padInt.URL);
        }

        // Adds or updates the value identified by the given uid
        private void AddValue(int uid, int value)
        {
            if (values.ContainsKey(uid))
                values[uid] = value;
            else
                values.Add(uid, value);
        }

        internal void addAccessedServer(string url)
        {
            if (!accessedServers.Contains(url))
                accessedServers.Add(url);
        }
    }
}
