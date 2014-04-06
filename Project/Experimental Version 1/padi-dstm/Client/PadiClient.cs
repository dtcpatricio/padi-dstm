using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Threading;
using padi_dstm_library;

namespace Client
{
    class PadiClient : Library
    {
        Form1 myForm = null;
        Library lib;

        // true if the portChannel is already defined, false otherwise
        bool portChannel;

        PadInt pi;

        public PadiClient(Form1 form)
        {
            myForm = form;
            portChannel = false;
        }

        public void setChannel()
        {
            if (portChannel == false)
            {
                TcpChannel channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, false);

                lib = (Library)Activator.GetObject(typeof(Library), "tcp://localhost:8086/Master");
                portChannel = true;
            }
        }

        public PadInt CreatePadInt(int uid)
        {
            pi = lib.CreatePadInt(uid);
            return pi;
        }

        public PadInt AccessPadInt(int uid)
        {
            pi = lib.AccessPadInt(uid);
            return pi;
        }

        public int ReadPadInt()
        {
            return pi.Read();
        }
    }
}
