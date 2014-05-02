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

        private static string _myURL;

        private static string _myPort;

        private String _logPath;

        private String _logFile = "log.txt";

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

        public static String MY_URL
        {
            get { return _myURL; }
            set { _myURL = value; }
        }

        public static String MY_PORT
        {
            get { return _myPort; }
            set { _myPort = value; }
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
            setLogPath();
            try
            {
                if (Directory.Exists(LOG_PATH))
                {
                    Console.WriteLine("The path " + LOG_PATH + " already exists.");
                    return;
                }

                DirectoryInfo di = Directory.CreateDirectory(LOG_PATH);
                Console.WriteLine("The directory was created successfully at {0}.", LOG_PATH);
                createLogFile();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0]", e.ToString());
            }
        }

        internal void setLogPath()
        {
            LOG_PATH = "C:\\PADI-DSTM\\" + TX.TXID + "\\" + initializePort();
        }

        internal string initializePort()
        {
            string[] url = MY_URL.Split('/');
            string localhost_port = url[url.Length - 2];
            string[] port = localhost_port.Split(':');
            MY_PORT = port[port.Length - 1];
            return MY_PORT;
        }


        internal void createLogFile()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(LOG_PATH + "\\" + _logFile);
            file.Close();
        }

        internal void writeAheadLog()
        {
            string text = System.IO.File.ReadAllText(LOG_PATH + "\\" + _logFile);
            text += "TEMP\r\n";
            
            foreach (ServerObject so in TX.WRITTENOBJECTS)
            {
                text += so.UID + " " + so.VALUE + " ";
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(LOG_PATH + "\\" + _logFile);
            file.WriteLine(text);
            file.Close();
        }

        internal void writePermanentLog()
        {
            string text = System.IO.File.ReadAllText(LOG_PATH + "\\" + _logFile);
            if (text.Contains("TEMP\r\n"))
            {
                text = text.Replace("TEMP\r\n", "");
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(LOG_PATH + "\\" + _logFile);
            file.WriteLine(text);
            file.Close();
        }

        internal void endTimer()
        {
            //TIMER.Enabled = false;
        }
    }
}
