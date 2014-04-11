using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace MasterServer
{
    class MasterServerMain
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(WorkerRegister),
                "WorkerRegister",
                WellKnownObjectMode.Singleton);

            System.Console.WriteLine("Registered Master Server on tcp://localhost:8086/WorkerRegister");

            System.Console.WriteLine("Master Server running");
            System.Console.ReadLine();

        }
    }
}
