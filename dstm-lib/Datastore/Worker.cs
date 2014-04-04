using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace Datastore
{
    class Worker
    {
        static void Main(string[] args)
        {
            System.Console.Write("Type in this worker server port: ");
            string port = System.Console.ReadLine();

            TcpChannel channel = new TcpChannel(Convert.ToInt32(port));
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemotePadInt),
                "RemotePadInt",
                WellKnownObjectMode.Singleton);

            System.Console.WriteLine("Registered RemotePadInt on tcp://localhost:" + port + "/RemotePadInt");


            // Quit message and waits for a key press
            System.Console.WriteLine("<enter> to kill the server");
            System.Console.ReadLine();

        }
    }
}
