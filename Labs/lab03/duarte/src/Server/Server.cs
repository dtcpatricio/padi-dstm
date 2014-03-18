using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using ChatApplicationInterfaces;

namespace ChatApplication {

	class Server {

		static void Main(string[] args) {

            // Open and bind the server channel for communication
			TcpChannel channel = new TcpChannel(8086);
			ChannelServices.RegisterChannel(channel,true);

            // Register the server object in the server domain
			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof(ChatRoom),
				"ChatRoom",
				WellKnownObjectMode.Singleton);
      
			System.Console.WriteLine("<enter> para sair...");
			System.Console.ReadLine();
		}

              
	}

    class ChatRoom : MarshalByRefObject, ICS
    {

        private Dictionary<string, string> nicks = new Dictionary<string, string>();

        public void Reg(string nick, string URL)
        {
            nicks.Add(nick, URL);
        }

        public void SendMsg(string nick, string msg)
        {
            ICC client;

            foreach (string n in nicks.Keys)
            {
                if (n.Equals(nick))
                    continue;

                client = (ICC)Activator.GetObject(
                typeof(ICC),
                nicks[n]);

                client.StoCMsg(nick, msg);
            }
        }
    }
}