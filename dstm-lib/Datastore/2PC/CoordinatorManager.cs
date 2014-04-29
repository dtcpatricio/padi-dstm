using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.Remoting.Messaging;
using System.IO;

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

        private String logPath;

        internal CoordinatorManager(List<String> URLs)
        {
            _myDecision = TransactionDecision.DEFAULT;
            initializeParticipants(URLs);
            logPath = @"C:\" + _tx.TXID;
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
        ~CoordinatorManager()
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

        public delegate void RemoteAsyncDelegate(int txID, string url);


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
                //participant.canCommit(_tx.TXID, Datastore.SERVERURL);

                // Create delegate to remote method
                RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(participant.canCommit);
                // Call delegate to remote method
                IAsyncResult RemAr = RemoteDel.BeginInvoke(_tx.TXID, Datastore.SERVERURL, null, null);
                // Wait for the end of the call and then explictly call EndInvoke
                //RemAr.AsyncWaitHandle.WaitOne();
                //Console.WriteLine(RemoteDel.EndInvoke(RemAr));
            }
            // First phase of commit, temporary write to disk
            writeAheadLog();

            timer();
            _myDecision = waitParticipantsResponse();
            evaluateMyDecision();

            if (_myDecision.Equals(TransactionDecision.COMMIT))
                writePermanentLog();
        }

        internal TransactionDecision waitParticipantsResponse()
        {
            while (true)
            {
                int noResponses = 0;
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
        }

        internal void evaluateMyDecision()
        {
            if(_myDecision.Equals(TransactionDecision.COMMIT))
                commit();
            
            if(_myDecision.Equals(TransactionDecision.ABORT))
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

        // TODO: Add same code to participants and create a new class that deals with this
        internal void writeAheadLog()
        {
            String log = "TEMP\n";

            try
            {
                if (Directory.Exists(logPath))
                {
                    Console.WriteLine("That path already exists.");
                    return;
                }

                DirectoryInfo di = Directory.CreateDirectory(logPath);
                Console.WriteLine("The directory was created successfully at {0}.", logPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0]", e.ToString());
            }

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

    }
}
