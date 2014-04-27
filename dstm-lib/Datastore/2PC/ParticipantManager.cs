using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{

    // Replacing TransactionManager - participant part
    class ParticipantManager
    {
        // not sure if necessary
        private static TentativeTx _tx;
        
        private TransactionDecision _myDecision;

        private String _coordinatorURL;

        internal String COORDINATORURL
        {
            get { return _coordinatorURL; }
            set { _coordinatorURL = value; }
        }

        internal ParticipantManager()
        {
            _myDecision = TransactionDecision.DEFAULT;
        }

        // TODO: Necessary because a server can be a coordinator as well as a participant
        internal ~ParticipantManager()
        {
            _tx = null;
            _myDecision = TransactionDecision.DEFAULT;
        }

        internal bool canCommit()
        {
            ICoordinator coord = (ICoordinator)Activator.GetObject(typeof(ICoordinator), _coordinatorURL);
            // TODO: test if transaction can commit
            coord.sendYes(_tx.TXID, Datastore.SERVERURL);

            _myDecision = TransactionDecision.COMMIT;
            // TODO: Iniciar timer para ficar a espera de commit ou abort final de coordenador
            // Se timer expirar abortar transacao
            return true; // placeholder
        }

        // Only for participants
        internal void doCommit(int txID, string coordURL)
        {
            // changes of tentative tx made permanent
            // terminar timer
        }

        // Only for participants
        internal void doAbort(int txID, string coordURL)
        {
            _myDecision = TransactionDecision.ABORT;
            // delete tentative tx
            // terminar timer
        }

        internal void getDecision()
        {
            // Hipotese: Se timer expirar, perguntar ao coordenador decisao final
            ICoordinator coord = (ICoordinator)Activator.GetObject(typeof(ICoordinator), _coordinatorURL);
            coord.haveCommitted(_tx.TXID, Datastore.SERVERURL);
            // save decision
        }
    }
}
