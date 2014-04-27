using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    public enum TransactionDecision
    {
        DEFAULT,    // without decision
        COMMIT,
        ABORT
    }
}
