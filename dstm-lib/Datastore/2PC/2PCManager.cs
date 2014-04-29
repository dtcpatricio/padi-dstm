using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Datastore
{
    // Class responsible for maintenance of participants and coordinators
    abstract class _2PCManager
    {
        private static TentativeTx _tx;

        private static TransactionDecision _myDecision;

        private String _logPath;

        // Timer
        //  - for participant: waiting for the coordinators response
        //  - for coordinator: waiting for all participant responses
        private System.Timers.Timer _timer;

        public static TentativeTx TX
        {
            get { return _tx; }
            set { _tx = value; }
        }

        public TransactionDecision MY_DECISION
        {
            get { return _myDecision; }
            set { _myDecision = value; }
        }

        public String LOG_PATH
        {
            get { return _logPath; }
            set { _logPath = value; }
        }

        public System.Timers.Timer TIMER
        {
            get { return _timer; }
            set { _timer = value; }
        }

        internal void createLogDirectory()
        {
            try
            {
                if (Directory.Exists(LOG_PATH))
                {
                    Console.WriteLine("That path already exists.");
                    return;
                }

                DirectoryInfo di = Directory.CreateDirectory(LOG_PATH);
                Console.WriteLine("The directory was created successfully at {0}.", LOG_PATH);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0]", e.ToString());
            }
        }

        internal void writeAheadLog()
        {
            String log = "TEMP\r\n";

            foreach (ServerObject so in TX.WRITTENOBJECTS)
            {
                log += so.UID + " " + so.VALUE + " ";
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(LOG_PATH + "\\WALparticipant.txt");
            file.WriteLine(log);

            file.Close();
        }

        internal void writePermanentLog()
        {
            string text = System.IO.File.ReadAllText(LOG_PATH + "\\WALparticipant.txt");
            if (text.Contains("TEMP\r\n"))
            {
                text.Replace("TEMP\r\n", "");
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(LOG_PATH + "\\WALparticipant.txt");
            file.WriteLine(text);
            file.Close();
        }

        internal void endTimer()
        {
            TIMER.Enabled = false;
        }
    }
}
