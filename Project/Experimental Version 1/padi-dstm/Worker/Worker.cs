using System;
using System.Collections;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace WorkerServer
{
    class Worker
    {
        static void Main(string[] args)
        {
            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            
            //Console.Write("My port: ");
            //int port = Convert.ToInt32(Console.ReadLine());
            
            props["port"] = 8087;
            TcpChannel channel = new TcpChannel(props, null, provider);
            //TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, false);
            PadIntWorker mo = new PadIntWorker();
            RemotingServices.Marshal(mo, "Server", typeof(PadIntWorker));
            System.Console.WriteLine("WORKER SERVER\n<enter> para sair...");
            System.Console.ReadLine();
        }
    }
}
