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
    class ParticipantManager
    {
        // not sure if necessary
        private static TentativeTx _tx;
        
        private static TransactionDecision _myDecision;

        private static TransactionDecision _myCoordinatorDecision;

        private static String _coordinatorURL;

        // Timer for waiting for the coordinators response
        private  System.Timers.Timer waitResponse;

        internal String COORDINATORURL
        {
            get { return _coordinatorURL; }
            set { _coordinatorURL = value; }
        }

        internal ParticipantManager()
        {
            _myDecision = TransactionDecision.DEFAULT;
            _myCoordinatorDecision = TransactionDecision.DEFAULT;
        }

        // TODO: Necessary because a server can be a coordinator as well as a participant
        ~ParticipantManager()
        {
            _tx = null;
            _myDecision = TransactionDecision.DEFAULT;
        }

        internal void timer()
        {
            // Create a timer with a ten second interval.
            waitResponse = new System.Timers.Timer(5000);

            // Hook up the event handler for the Elapsed event.
            waitResponse.Elapsed += new ElapsedEventHandler(getDecision);
            
            // Only raise the event the first time Interval elapses.
            waitResponse.AutoReset = false;
            waitResponse.Enabled = true;
        }

        internal void endTimer()
        {
            waitResponse.Enabled = false;
        }

        internal void canCommit()
        {
            ICoordinator coord = (ICoordinator)Activator.GetObject(typeof(ICoordinator), _coordinatorURL);
            // TODO: test if transaction can commit
            
            _myDecision = TransactionDecision.COMMIT;

            writeAheadLog();

            timer();
            coord.sendYes(_tx.TXID, Datastore.SERVERURL);

            _myDecision = waitForCoordinator();
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

        internal void writeAheadLog()
        {
            String log = "TEMP\n";

            foreach (ServerObject so in _tx.WRITTENOBJECTS)
            {
                log += so.UID + " " + so.VALUE + " ";
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\WALparticipant.txt");
            file.WriteLine(log);

            file.Close();
        }

        internal void writePermanentLog()
        {
            string text = System.IO.File.ReadAllText(@"C:\\WALparticipant.txt");
            if (text.Contains("TEMP\n"))
            {
                text.Replace("TEMP\n", "");
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\WALparticipant.txt");
            file.WriteLine(text);
            file.Close();
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
            _myDecision = TransactionDecision.ABORT;
            
            // delete tentative tx, é necessário apagar a lista de written objects?

            if (File.Exists(@"C:\WALparticipant.txt"))
            {
                File.Delete(@"C:\WALparticipant.txt");
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
            coord.haveCommitted(_tx.TXID, Datastore.SERVERURL);
            // save decision
        }
    }
}
