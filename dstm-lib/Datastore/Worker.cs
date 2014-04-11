using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using CommonTypes;

namespace Datastore
{
    class Worker
    {
        public static string _worker_url;

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

            // To register worker server
            IWorkerRegister workerManager = (IWorkerRegister)Activator.GetObject(
                    typeof(IWorkerRegister), "tcp://localhost:8086/WorkerRegister");
            _worker_url = "tcp://localhost:" + port + "/RemotePadInt";

            workerManager.registerWorker(_worker_url);
            System.Console.WriteLine("Registered RemotePadInt on " + _worker_url);

            // Quit message and waits for a key press
            System.Console.WriteLine("<enter> to kill the server");
            System.Console.ReadLine();

        }
    }
}
