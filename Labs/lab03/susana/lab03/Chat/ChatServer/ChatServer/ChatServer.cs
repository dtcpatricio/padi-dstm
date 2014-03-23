using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using ChatLib;

namespace ChatServer
{
    class ChatServer
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);

            Chat chat = new Chat();
            RemotingServices.Marshal(chat, "ChatRoom", typeof(Chat));

            System.Console.WriteLine("<enter> para sair...");
            System.Console.ReadLine();
        }
    }
}
