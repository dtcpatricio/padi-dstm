using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using ChatLib;

namespace ChatServer
{
    public class Chat : MarshalByRefObject, InterfaceChatServer
    {
        Dictionary<string,string> users = new Dictionary<string, string>();

        public bool validateNick(string nick)
        {
            if (users.ContainsKey(nick))
            {
                return true;
            }
            return false;
            
        }

        public string invalidNick(string nick)
        {
            if (validateNick(nick))
            {
                return "";
            }
            return "Nick " + nick + " already exists.";
        }

        public bool validateURL(string url)
        {
            if (users.ContainsValue(url))
            {
                return true;
            }
            return false;
        }

        public string invalidURL(string url) {
            if (validateURL(url))
            {
                return "";
            }
            return "URL " + url + " already exists.";
        }

        public void register(string nick, string url)
        {
            String clientMessage = "";
            if (validateNick(nick) || validateURL(url))
            {
                clientMessage = "ERROR: " + "\t" + invalidNick(nick) + invalidURL(url);
                System.Console.WriteLine(clientMessage);
            }
            else
            {
                users.Add(nick, url);
                clientMessage = nick + " added to conversation";
                System.Console.WriteLine(clientMessage + " through " + url);
                sendMessageAllClients(nick, clientMessage);
            }
        }

        public void sendMessageAllClients(string nick, string message)
        {
            InterfaceChatClient client = null;

            foreach (KeyValuePair<string, string> user in users)
            {
                if (!user.Key.Equals(nick))
                {
                    client = (InterfaceChatClient)Activator.GetObject(
                            typeof(InterfaceChatClient),
                            user.Value);

                    client.serverToClientMessage(message);
                }
            }
        }

        /* Sends message to all registered clients */
        public void sendMessage(string nick, string message)
        {
            sendMessageAllClients(nick, nick + " > " + message);
            
        } 
    }
}
