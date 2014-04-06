using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Threading;
using padi_dstm_library;

namespace MasterServer
{
    class PadIntManager : MarshalByRefObject, Library
    {
        // Key: int uid
        // Value: url of worker server where padInt with uid will be inserted
        private IDictionary<int, string> workers;

        // true if the portChannel is already defined, false otherwise
        bool portChannel;

        Library lib;

        public PadIntManager()
        {
            workers = new Dictionary<int, string>();
            portChannel = false;
        }

        public PadInt CreatePadInt(int uid)
        {
            if (workers.ContainsKey(uid))
            {
                Console.WriteLine("PadInt with uid " + uid + " already exists.");
                return null;
            }

            string url = setWorkerChannel();
            AddPadInt(uid, url);
            Console.WriteLine("PadInt Created with uid: " + uid + " on url: " + url + ".");
            return lib.CreatePadInt(uid);
        }

        public void AddPadInt(int uid, string url)
        {
            workers.Add(uid, url);
        }

        public string setWorkerChannel()
        {
            string url = "tcp://localhost:8087/Server";

            if (portChannel == false)
            {
               // TcpChannel channel = new TcpChannel();
                //ChannelServices.RegisterChannel(channel, false);

                lib = (Library)Activator.GetObject(typeof(Library), url);
                portChannel = true;
            }
            
            return url;
        }

        public PadInt AccessPadInt(int uid)
        {
            if (workers.ContainsKey(uid))
            {
                string url = setWorkerChannel();
                Console.WriteLine("PadInt Accessed with uid: " + uid + " on url: " + url + ".");
                return lib.AccessPadInt(uid);
            }
            else
            {
                Console.WriteLine("PadInt with uid " + uid + " does not exist.");
                return null;
            }
        }
    }
}
