using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using CommonTypes.DatastoreMaster;

namespace Datastore
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.Write("Type in this worker server port: ");
            string port = System.Console.ReadLine();

            string masterURL = "tcp://localhost:8086/";
            string datastoreURL = "tcp://localhost:" + port + "/";

            // register the TCP channel
            TcpChannel channel = new TcpChannel(Convert.ToInt32(port));
            ChannelServices.RegisterChannel(channel, true);

            // register runtime services:
            // - RemotePadInt
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemotePadInt),
                "RemotePadInt",
                WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Registered RemotePadInt on tcp://localhost:" + port + "/RemotePadInt");

            // - DatastoreOps
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(DatastoreOps),
                "DatastoreOps",
                WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Registered DatastoreOps on tcp://localhost:" + port + "/DatastoreOps");

            // TODO: Not sure if creation of 2 services corresponding to participant and coordinator, or 
            // integration with already specified services (RemotePadInt or DatastoreOps)
            // Participant or coordinator
            RemotingConfiguration.RegisterWellKnownServiceType(
               typeof(Participant),
               "Participant",
               WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Registered Coordinator on tcp://localhost:" + port + "/Participant");

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Coordinator),
                "Coordinator",
                WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Registered Coordinator on tcp://localhost:" + port + "/Coordinator");

            // register the datastore server on the master server
            IDatastoreComm master = (IDatastoreComm)Activator.GetObject(
                typeof(IDatastoreComm),
                masterURL + "DatastoreComm");
            bool success = master.registerWorker(datastoreURL);

            if (!success)
            {
                // kill server
            }

            Datastore.SERVERURL = datastoreURL;
            // Quit message and waits for a key press
            System.Console.WriteLine("<enter> to kill the server");
            System.Console.ReadLine();

        }
    }
}
