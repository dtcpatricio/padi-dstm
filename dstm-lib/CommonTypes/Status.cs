using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    // All nodes in the system dump to their output
    // their current state
    // TODO: implement in nodes, what's node's state
    abstract class Status
    {
        // Status is defined as a string
        private string status;

        public string getStatus()
        {
            return status;
        }
    }
}
