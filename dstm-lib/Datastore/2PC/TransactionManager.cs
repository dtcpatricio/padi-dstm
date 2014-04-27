using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Datastore
{
    internal class TransactionManager
    {
        private static List<String> _URLs;
        private static TentativeTx _tx;
        private static Dictionary<String, ParticipantResponse> _participantsResponse;

        private DecisionState _myDecision;

        private System.Timers.Timer aTimer;

        // coordinator constructor
        internal  TransactionManager(TentativeTx tx, List<String> URLs)
        {
            _tx = tx;
            _URLs = URLs;
            _participantsResponse = new Dictionary<string, ParticipantResponse>();

            foreach (String url in _participantsResponse.Keys)
            {
                _participantsResponse[url] = ParticipantResponse.NORESPONSE;
            }
        }

        // participant constructor
        internal TransactionManager(TentativeTx tx)
        {
            _tx = tx;
        }


        /* Participant response to Yes in canCommit*/
        internal void voteYes(String url)
        {
            _participantsResponse[url] = ParticipantResponse.YES;
        }

        /* Participant response to No in canCommit */
        internal void voteNo(String url)
        {
            _participantsResponse[url] = ParticipantResponse.NO; //Note: Not really necessary

            // Do abort for all participants
            
        }

        internal void addTransactionURLs(TentativeTx tx, List<String> URLs)
        {
            _tx = tx;
            _URLs = URLs;
        }

        internal void addTransaction(TentativeTx tx)
        {
            _tx = tx;
            _URLs = null; /* PRECISA DE SER NULL */
        }

        internal static void onTimeAbort(object source, ElapsedEventArgs e)
        {
            foreach (String url in _URLs)
            {
                // TODO: A thread per participant
                IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                participant.doAbort(_tx.TXID, Datastore.SERVERURL);
            }
        }

        internal void timer()
        {
            // Create a timer with a ten second interval.
            aTimer = new System.Timers.Timer(5000);

            // Hook up the event handler for the Elapsed event.
            aTimer.Elapsed += new ElapsedEventHandler(onTimeAbort);

            // Only raise the event the first time Interval elapses.
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
        }

        // TODO: should run a separate thread to receive replies
        // only the coordinator should call this
        internal void prepare()
        {
            foreach (String url in _URLs)
            {
                // TODO: A thread per participant
                IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                participant.canCommit(_tx.TXID, Datastore.SERVERURL);
            }

            timer();
            commit();
        }

        // only the participant should call this
        internal bool canCommit()
        {
            return true; // placeholder
        }

        internal void commit()
        {
            foreach (String url in _URLs)
            {
                // TODO: A thread per participant
                IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                participant.doCommit(_tx.TXID, Datastore.SERVERURL);
            }
        }

    }
}
