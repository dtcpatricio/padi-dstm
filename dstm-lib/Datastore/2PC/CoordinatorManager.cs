using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Datastore
{
    // Replacing TransactionManager - coordinator part
    internal class CoordinatorManager
    {
        // not sure if necessary
        private static TentativeTx _tx;
        
        private TransactionDecision _myDecision;

        // participants URL's
       //private static List<String> _URLs;
        
        private static Dictionary<String, ParticipantResponse> _participantURLs;
        
        private System.Timers.Timer participantsTimer;

        internal CoordinatorManager(List<String> URLs)
        {
            _myDecision = TransactionDecision.DEFAULT;
            initializeParticipants(URLs);
        }

        internal void initializeParticipants(List<String> URLs)
        {
            _participantURLs = new Dictionary<String, ParticipantResponse>();
            foreach (String url in URLs)
            {
                _participantURLs.Add(url, ParticipantResponse.NORESPONSE);
            }
        }

        // TODO: Necessary because a server can be a coordinator as well as a participant
        internal ~CoordinatorManager()
        {
            _tx = null;
            _myDecision = TransactionDecision.DEFAULT;
            _participantURLs = null;
        }

        internal static void onTimeAbort(object source, ElapsedEventArgs e)
        {
            foreach (String url in _participantURLs.Keys)
            {
                // TODO: A thread per participant
                IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                participant.doAbort(_tx.TXID, Datastore.SERVERURL);
            }
        }

        internal void timer()
        {
            // Create a timer with a ten second interval.
            participantsTimer = new System.Timers.Timer(5000);

            // Hook up the event handler for the Elapsed event.
            participantsTimer.Elapsed += new ElapsedEventHandler(onTimeAbort);

            // Only raise the event the first time Interval elapses.
            participantsTimer.AutoReset = false;
            participantsTimer.Enabled = true;
        }

        // TODO: should run a separate thread to receive replies
        // only the coordinator should call this
        internal void prepare()
        {
            foreach (String url in _participantURLs.Keys)
            {
                // TODO: A thread per participant
                // after all thread are sent, create one main thread responsible
                // for receiving votes (yes or no) - Assync callback (ver exemplo da aula 4) 
                // and another one to run the timer
                IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                if (participant.canCommit(_tx.TXID, Datastore.SERVERURL))
                {
                    // can commit
                }
                else
                {
                    // cannot commit
                }
            }

            //timer();
        }

        internal void commit()
        {
            // If all participants responded 'yes'
            foreach (String url in _participantURLs.Keys)
            {
                // TODO: A thread per participant
                IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                participant.doCommit(_tx.TXID, Datastore.SERVERURL);
            }
        }

        internal void abort()
        {
            // If at least one participant responded 'no'
            foreach (String url in _participantURLs.Keys)
            {
                // TODO: A thread per participant
                IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                participant.doAbort(_tx.TXID, Datastore.SERVERURL);
            }
        }

        internal void participantYes(String url)
        {
            _participantURLs[url] = ParticipantResponse.YES;
        }

        internal void participantNo(String url)
        {
            _participantURLs[url] = ParticipantResponse.NO;
            // TODO: send abort to all participants, and abort transaction
        }

        internal bool haveCommitted(String url)
        {
            // TODO: confirm if transaction has committed or not
            return true;
        }
    }
}
