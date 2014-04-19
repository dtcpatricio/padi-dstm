using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    internal interface IParticipant
    {
        void canCommit(int txID, string coordURL);
        void doCommit(int txID, string coordURL);
    }
}
