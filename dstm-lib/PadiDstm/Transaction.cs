using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using CommonTypes.LibraryMaster;
using CommonTypes.LibraryDatastore;

namespace PADI_DSTM
{
    public class Transaction
    {
        private int _txID;
        internal TransactionState State { get; private set; }
        private TcpChannel channel;

        // maps padint uids with the values
        private Dictionary<int, int> values = new Dictionary<int, int>();

        // list of the servers accessed within the transaction
        private List<string> accessedServers = new List<string>();

        internal int TXID { get { return _txID; } }

        // Returns the dictionary of all the values (uid, value) kept in the current transaction
        internal Dictionary<int, int> GetValues { get { return this.values; } }

        // returns the list of the accessed servers throughout the transaction
        internal List<string> ACCESSEDSERVERS { get { return accessedServers; } }

        internal Transaction()
        {
            // open a tcp channel to register the TransactionValues object
            // the port definition strategy needs to be taken into account
            channel = new TcpChannel(Convert.ToInt32(PadiDstm.Port));
            ChannelServices.RegisterChannel(channel, true);

            // register the EndTransaction object
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(EndTransaction),
                "EndTransaction",
                WellKnownObjectMode.Singleton);

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


        internal int Read(PadInt padInt)
        {
            int val;
            
            string remotePadIntURL = padInt.URL + "RemotePadInt";
            int uid = padInt.UID;

            IRemotePadInt remote = (IRemotePadInt)Activator.GetObject(
                typeof(IRemotePadInt),
                remotePadIntURL);

            // call the RemotePadInt to get the value
            val = remote.Read(uid, TXID, PadiDstm.Client_Url);

            AddValue(uid, val);

            if (!accessedServers.Contains(padInt.URL))
                accessedServers.Add(padInt.URL);

            return val;
        }

        internal void Write(PadInt padInt, int val)
        {
            string remotePadIntURL = padInt.URL + "RemotePadInt";
            int uid = padInt.UID;

            IRemotePadInt remote = (IRemotePadInt)Activator.GetObject(
                typeof(IRemotePadInt),
                remotePadIntURL);

            remote.Write(uid, TXID, val, PadiDstm.Client_Url);
            AddValue(uid, val);

            if (!accessedServers.Contains(padInt.URL))
                accessedServers.Add(padInt.URL);
        }

        // Adds or updates the value identified by the given uid
        private void AddValue(int uid, int value)
        {
            if (values.ContainsKey(uid))
                values[uid] = value;
            else
                values.Add(uid, value);
        }

        internal void closeChannel()
        {
            channel.StopListening(null);
            ChannelServices.UnregisterChannel(channel);
            channel = null;
        }
    }
}
