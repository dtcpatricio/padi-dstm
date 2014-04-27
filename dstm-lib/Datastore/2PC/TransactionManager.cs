using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    internal class TransactionManager
    {
        private List<String> _URLs;
        private TentativeTx _tx;

        // coordinator constructor
        internal TransactionManager(TentativeTx tx, List<String> URLs)
        {
            _tx = tx;
            _URLs = URLs;
        }

        // participant constructor
        internal TransactionManager(TentativeTx tx)
        {
            _tx = tx;
        }

        // TODO: should run a separate thread to receive replies
        // only the coordinator should call this
        internal void prepare()
        {
            foreach (String url in _URLs)
            {
                IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                participant.canCommit(_tx.TXID, Datastore.SERVERURL);
            }
        }

        // only the participant should call this
        internal bool canCommit()
        {
            return true; // placeholder
        }
    }
}
