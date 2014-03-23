using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib
{
    public interface InterfaceChatServer
    {
        /* Client invokes sendMessage in server to send message */
        void sendMessage(string nick, string message);

        /* Client invokes register in server to register its url */
        void register(string nick, string url);
    }
}
