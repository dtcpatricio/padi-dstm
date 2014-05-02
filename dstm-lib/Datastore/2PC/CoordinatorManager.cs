using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.Runtime.Remoting.Messaging;
using System.IO;

namespace Datastore
{
    // Replacing TransactionManager - coordinator part
    internal class CoordinatorManager : _2PCManager
    {
        private static Dictionary<String, ParticipantResponse> _participantURLs;

        private int _number_responses = 0;

        private static bool _allResponses = false;

        internal CoordinatorManager(TentativeTx tx, List<String> URLs)
        {
            MY_URL = Datastore.SERVERURL + "Coordinator";
            TX = tx;
            MY_DECISION = TransactionDecision.DEFAULT;
            initializeParticipants(URLs);
            createLogDirectory();
        }

        internal void initializeParticipants(List<String> URLs)
        {
            _participantURLs = new Dictionary<String, ParticipantResponse>();
            foreach (String url in URLs ?? new List<String>())
            {
                _participantURLs.Add(url + "Participant", ParticipantResponse.NORESPONSE);
            }
        }

        // TODO: Necessary because a server can be a coordinator as well as a participant
        ~CoordinatorManager()
        {
        }

        internal static void onTimeAbort(object source, ElapsedEventArgs e)
        {
            foreach (String url in _participantURLs.Keys)
            {
                // TODO: A thread per participant
                IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                participant.doAbort(TX.TXID, MY_URL);
            }
        }

        internal void timer()
        {
            // Create a timer with a ten second interval.
            TIMER = new System.Timers.Timer(10000);

            // Hook up the event handler for the Elapsed event.
            TIMER.Elapsed += new ElapsedEventHandler(onTimeAbort);

            // Only raise the event the first time Interval elapses.
            TIMER.AutoReset = false;
            TIMER.Enabled = true;
        }

        public delegate void RemoteAsyncDelegate(int txID, string url);


        // TODO: should run a separate thread to receive replies
        // only the coordinator should call this
        internal void prepare()
        {
            Console.WriteLine("I'm the coordinator: " + MY_URL);
            if (_participantURLs.Count > 0)
            {
                foreach (String url in _participantURLs.Keys)
                {
                    // TODO: A thread per participant
                    // after all thread are sent, create one main thread responsible
                    // for receiving votes (yes or no) - Assync callback (ver exemplo da aula 4) 
                    // and another one to run the timer
                    Console.WriteLine("URL: " + url);
                    IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                    //participant.canCommit(TX.TXID, MY_URL);

                    // Create delegate to remote method
                    RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(participant.canCommit);
                    // Call delegate to remote method
                    IAsyncResult RemAr = RemoteDel.BeginInvoke(TX.TXID, MY_URL, null, null);
                    // Wait for the end of the call and then explictly call EndInvoke
                    //RemAr.AsyncWaitHandle.WaitOne();
                    //Console.WriteLine(RemoteDel.EndInvoke(RemAr));
                }
                //timer();
                MY_DECISION = waitParticipantsResponse();
                evaluateMyDecision();
            }
            else
            {
                // Coordinator decision
                // Default commit
                MY_DECISION = TransactionDecision.COMMIT;
                Console.WriteLine("AFTER MY_DECISION is commit and there are no participants");
            }
            // First phase of commit, temporary write to disk
            writeAheadLog();

            if (MY_DECISION.Equals(TransactionDecision.COMMIT))
                writePermanentLog();
        }

        internal TransactionDecision waitParticipantsResponse()
        {
            int noResponses = 0;
            lock (this)
            {
                if(!_allResponses)
                    Monitor.Wait(this);
                
                foreach (String url in _participantURLs.Keys)
                {
                    if (_participantURLs[url].Equals(ParticipantResponse.NORESPONSE))
                        noResponses++;

                    if (_participantURLs[url].Equals(ParticipantResponse.NO))
                        return TransactionDecision.ABORT;

                    if (noResponses <= 0)
                        return TransactionDecision.COMMIT;
                }
            }
            return TransactionDecision.DEFAULT;
        }

        internal void receivedAllResponses()
        {
            lock (this)
            {
                _number_responses++;
                if (_number_responses == _participantURLs.Count)
                {
                    _allResponses = true;
                    Monitor.PulseAll(this);
                }
            }
        }

        internal void evaluateMyDecision()
        {
            if(MY_DECISION.Equals(TransactionDecision.COMMIT))
                commit();
            
            if(MY_DECISION.Equals(TransactionDecision.ABORT))
                abort();

            // Should return transaction exception
        }

        internal void commit()
        {
            // If all participants responded 'yes'
            foreach (String url in _participantURLs.Keys)
            {
                // TODO: A thread per participant
                IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                //participant.doCommit(TX.TXID, MY_URL);
                RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(participant.doCommit);
                // Call delegate to remote method
                IAsyncResult RemAr = RemoteDel.BeginInvoke(TX.TXID, MY_URL, null, null);
            }
        }

        internal void abort()
        {
            // If at least one participant responded 'no'
            foreach (String url in _participantURLs.Keys)
            {
                // TODO: A thread per participant
                IParticipant participant = (IParticipant)Activator.GetObject(typeof(IParticipant), url);
                participant.doAbort(TX.TXID, MY_URL);
            }
        }

        internal void participantYes(String url)
        {
            _participantURLs[url] = ParticipantResponse.YES;
            receivedAllResponses();
        }

        internal void participantNo(String url)
        {
            _participantURLs[url] = ParticipantResponse.NO;
            receivedAllResponses();
            // TODO: send abort to all participants, and abort transaction
        }

        internal bool haveCommitted(String url)
        {
            // TODO: confirm if transaction has committed or not
            return true;
        }
    }
}
