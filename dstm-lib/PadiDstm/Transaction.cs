using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using CommonTypes;

namespace Padi_dstm
{
    public class Transaction
    {

        public TransactionState State { get; private set; }

        public Transaction()
        {
            // open a tcp channel to register the TransactionValues object
            // the port definition strategy needs to be taken into account
            TcpChannel channel = new TcpChannel(Convert.ToInt32(PadiDstm.Port));
            ChannelServices.RegisterChannel(channel, true);

            // register the TransactionValues object
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(TransactionValues),
                "TransactionValues",
                WellKnownObjectMode.Singleton);

            // set the state of the transaction to ACTIVE
            State = TransactionState.ACIVE;
        }


        internal int Read(PadInt padInt)
        {
            string url = "tcp://localhost:" + PadiDstm.Port + "/TransactionValues";
            string remotePadIntURL = "tcp://localhost:8086" + "/RemotePadInt";
            //string remotePadIntURL = padInt.URL + "/RemotePadInt";
            int uid = padInt.UID;

            IRemotePadInt remote = (IRemotePadInt)Activator.GetObject(
                typeof(IRemotePadInt),
                remotePadIntURL);

            // call the RemotePadInt to get the value
            // the worker server must send the value to TransactionValue Singleton
            remote.Read(uid, url);

            return uid; // placeholder
            //throw new NotImplementedException();
        }

        internal void Write(PadInt padInt, int val)
        {
            string url = "tcp://localhost:" + PadiDstm.Port + "/TransactionValues";
            string remotePadIntURL = padInt.URL + "/RemotePadInt";
            int uid = padInt.UID;

            IRemotePadInt remote = (IRemotePadInt)Activator.GetObject(
                typeof(IRemotePadInt),
                remotePadIntURL);

            // maybe the new val could come from TransactionValue ...
            remote.Write(uid, val, url);
            //throw new NotImplementedException();
        }
    }
}
