using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using ChatApplicationInterfaces;

namespace ChatApplication {

	class Client {

        static void Main()
        {
            // Read client port
            Console.Write("Type in your port: ");
            string port = Console.ReadLine();

            // Open and bind the channel for communication
            TcpChannel channel = new TcpChannel(Convert.ToInt32(port));
            ChannelServices.RegisterChannel(channel, true);

            // Register client object
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ChatObject),
                "ChatObject",
                WellKnownObjectMode.Singleton);

            // Get server object
            ICS obj = (ICS)Activator.GetObject(
                typeof(ICS),
                "tcp://localhost:8086/ChatRoom");

            // Query the client's username
            Console.Write("Type username: ");
            string username = Console.ReadLine();

            try
            {
                regUser(username, port, obj);
                chat(username, obj);
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate server");
            }

        }

        /* Func Chat
         * Chat main loop
         * 
         * @param user String representing the username
         * @param obj The remote object held by the server
         */
        public static void chat(string user, ICS obj)
        {
            string msg;

            while (true)
            {
                Console.Write(user + "> ");
                msg = Console.ReadLine();
                obj.SendMsg(user, msg);
            }
        }

        /* Func regUser
         * A function to register a new user to the chat room
         * 
         * @param user String representing the username
         * @param port String of an integer representing the port this client is listening
         * @param obj The remote object held by the server
         */
        public static void regUser(string user, string port, ICS obj)
        {
            obj.Reg(user, "tcp://localhost:" + port + "/ChatObject");
        }
	}


    class ChatObject : MarshalByRefObject, ICC
    {
        public void StoCMsg(string user, string msg)
        {
            // Give a new-line space to avoid mixing usernames
            Console.WriteLine();

            Console.WriteLine(user + "> " + msg);
            /*mf.Invoke(new delAddLine(mf.AddLine),
                object[] { msg });*/
        }
    }
}