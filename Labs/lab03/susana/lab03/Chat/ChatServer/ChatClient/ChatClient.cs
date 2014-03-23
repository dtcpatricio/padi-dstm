using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Threading;
using ChatLib;

namespace ChatClient
{
    class ChatClient : MarshalByRefObject, InterfaceChatClient
    {
        InterfaceChatServer chat;
        string myURL;
        Form1 myForm = null;
        string nick;

        public ChatClient(Form1 form)
        {
            myForm = form;
        }

        public void setPortNumber(int portNumber)
        {
            TcpChannel channel = new TcpChannel(portNumber);
            ChannelServices.RegisterChannel(channel, true);

            RemotingServices.Marshal(this, "ChatClient", typeof(ChatClient));

            chat = (InterfaceChatServer)Activator.GetObject(
                typeof(InterfaceChatServer),
                "tcp://localhost:8086/ChatRoom");
        }

        public void serverToClientMessage(string message)
        {
            myForm.Invoke(myForm.myDelegate, new Object[] { message });
        }

        public string getNick()
        {
            return nick;
        }

        public void sendMessageToServer(string message)
        {
            chat.sendMessage(nick, message);
        }

        public void registerToServer(string nick, string portNumber)
        {
            int portNumberInt = Convert.ToInt32(portNumber);
            this.nick = nick;
            myURL = "tcp://localhost:" + portNumber + "/ChatClient";
            chat.register(nick, myURL);
        }
    }
}
