using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;

namespace Datastore
{

    // Replacing TransactionManager - participant part
    class ParticipantManager : _2PCManager
    {
        private static TransactionDecision _myCoordinatorDecision;

        private static String _coordinatorURL;

        internal String COORDINATORURL
        {
            get { return _coordinatorURL; }
            set { _coordinatorURL = value; }
        }

        internal TransactionDecision COORDINATOR_DECISION
        {
            get { return _myCoordinatorDecision; }
            set { _myCoordinatorDecision = value; }
        }

        internal ParticipantManager(TentativeTx tx)
        {
            TX = tx;
            MY_DECISION = TransactionDecision.DEFAULT;
            COORDINATOR_DECISION = TransactionDecision.DEFAULT;
            LOG_PATH = "C:\\" + TX.TXID;
            createLogDirectory();
        }

        // TODO: Necessary because a server can be a coordinator as well as a participant
        ~ParticipantManager()
        {
        }

        internal void timer()
        {
            // Create a timer with a ten second interval.
            TIMER = new System.Timers.Timer(5000);

            // Hook up the event handler for the Elapsed event.
            TIMER.Elapsed += new ElapsedEventHandler(getDecision);
            
            // Only raise the event the first time Interval elapses.
            TIMER.AutoReset = false;
            TIMER.Enabled = true;
        }

        internal void canCommit()
        {
            ICoordinator coord = (ICoordinator)Activator.GetObject(typeof(ICoordinator), _coordinatorURL);
            // TODO: test if transaction can commit
            
            MY_DECISION = TransactionDecision.COMMIT;

            writeAheadLog();

            timer();
            coord.sendYes(TX.TXID, Datastore.SERVERURL);

            MY_DECISION = waitForCoordinator();
        }

        internal TransactionDecision waitForCoordinator()
        {
            while (true)
            {
                if (!_myCoordinatorDecision.Equals(TransactionDecision.DEFAULT))
                {
                    return _myCoordinatorDecision;
                }
            }
        }

        


        // Only for participants
        internal void doCommit(int txID, string coordURL)
        {
            _myCoordinatorDecision = TransactionDecision.COMMIT;

            writePermanentLog();
            endTimer();
        }

        // Only for participants
        internal void doAbort(int txID, string coordURL)
        {
            MY_DECISION = TransactionDecision.ABORT;
            
            // delete tentative tx, é necessário apagar a lista de written objects?

            if (File.Exists(@LOG_PATH + "WALparticipant.txt"))
            {
                File.Delete(@LOG_PATH + "WALparticipant.txt");
            }
            endTimer();
        }

        internal static void getDecision(object source, ElapsedEventArgs e)
        {
            // Hipotese: Se timer expirar, perguntar ao coordenador decisao final
            ICoordinator coord = (ICoordinator)Activator.GetObject(typeof(ICoordinator), _coordinatorURL);

            //It is assumed that if the coordinator fails to respond, the transaction is 
            //Alternative strategies are available for the participants to obtain a decision 
            // cooperatively instead of contacting the coordinato
            coord.haveCommitted(TX.TXID, Datastore.SERVERURL);
            // save decision
        }
    }
}
