using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib
{
    public interface InterfaceChatClient
    {
        /* Server invokes this method to send message */
        void serverToClientMessage(string message);
    }
}
