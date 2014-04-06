using System;
using System.Collections;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace MasterServer
{
    class Master
    {
        static void Main(string[] args)
        {
            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            props["port"] = 8086;
            TcpChannel channel = new TcpChannel(props, null, provider);
            //TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, false);
            PadIntManager mo = new PadIntManager();
            RemotingServices.Marshal(mo, "Master", typeof(PadIntManager));
            System.Console.WriteLine("MASTER SERVER\n<enter> para sair...");
            System.Console.ReadLine();
        }
     }
}
