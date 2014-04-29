using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    internal class TentativeTx
    {
        private int _txID;
        private string _clientURL;

        // TODO: after commiting or aborting set _txDecision
        // PROBLEM: transaction is cached for a little to let participants
        // make getDecision call to know that tx aborted or committed?
        private TransactionDecision _txDecision;
        
        private List<ServerObject> writtenObjects;

        // Each TentativeTx has a coordinator or a participant
        // depending of the Datastore
        private CoordinatorManager _cm;
        private ParticipantManager _pm;

        internal List<ServerObject> WRITTENOBJECTS
        {
            get { return writtenObjects; }
        }

        internal CoordinatorManager COORDINATOR
        {
            get { return _cm; }
            set { _cm = value; }
        }
        internal ParticipantManager PARTICIPANT
        {
            get { return _pm; }
            set { _pm = value; }
        }

        internal int TXID { get { return _txID; } }
        internal string CLIENTURL { get { return _clientURL; } }

        internal TentativeTx(int txID, string clientURL)
        {
            _txID = txID;
            _clientURL = clientURL;
            writtenObjects = new List<ServerObject>();
            _txDecision = TransactionDecision.DEFAULT;
        }

        internal void AddObject(ServerObject obj)
        {
            writtenObjects.Add(obj);
        }
    }
}
