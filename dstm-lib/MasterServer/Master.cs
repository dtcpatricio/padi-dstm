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
    public class MasterServerMain
    {
        static void Main(string[] args)
        {
            // hard-typed port
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(DatastoreComm),
                "DatastoreComm",
                WellKnownObjectMode.Singleton);

            System.Console.WriteLine("Registered Master Server DatastoreComm object on:\r\ntcp://localhost:8086/DatastoreComm");

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(LibraryComm),
                "LibraryComm",
                WellKnownObjectMode.Singleton);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(WorkerAlive),
                "WorkerAlive",
                WellKnownObjectMode.Singleton);

            System.Console.WriteLine("Registered Master Server LibraryComm object on:\r\ntcp://localhost:8086/LibraryComm");

            System.Console.WriteLine("Master Server running...");
           
            //HeartBeat.timerAlive();
            
            System.Console.ReadLine();

        }
    }
}
