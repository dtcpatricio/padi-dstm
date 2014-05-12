using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;
using System.Timers;

namespace MasterServer
{
    internal static class HeartBeat
    {
        // Timer for each server
        static private Dictionary<string, Timer> _timerServers = new Dictionary<string, Timer>();

        // Normal time is 15 000 miliseconds -> 15s
        private const int TIMETOFAILURE = 15000;

        internal static Dictionary<string, Timer> TIMERSERVERS
        {
            get { return _timerServers; }
        }

        // TODO: Detect failure if worker fails to reply
        internal static Timer timerAlive(string server_url)
        {
            // Create a timer with a ten second interval.
            Timer timer = new Timer(TIMETOFAILURE);

            // Hook up the event handler for the Elapsed event.
            timer.Elapsed += (source, e) => onTimeFail(source, e, server_url);
            
            // Only raise the event the first time Interval elapses.
            timer.AutoReset = true;
            timer.Enabled = true;
            return timer;
        }

      
        // Called when master fails to receive a I AM ALIVE msg from a server
        internal static void onTimeFail(object source, ElapsedEventArgs e, string server_url)
        {
            Console.WriteLine("Server " + server_url + " Died!");
            _timerServers[server_url].Enabled = false;
            _timerServers[server_url].AutoReset = false;
            _timerServers.Remove(server_url);
            WorkerManager.setFailedServer(server_url);
        }
        
        internal static void resetTimer(Timer timer)
        {
            timer.Enabled = false;
            //timer.Interval = TIMETOFAILURE;
            timer.Interval = 15000;
            timer.AutoReset = true;
            timer.Enabled = true;
        }


        // Is there a need of lock?
        internal static void IAmAlive(String worker_url)
        {
            if (!WorkerManager.isFailedServer(worker_url))
            {
                
                Console.WriteLine("I Am ALIVE " + worker_url + "!");
                resetTimer(_timerServers[worker_url]);
            }
            else
            {
                // If the worker_url is not in the heartbeat servers it means
                // that it came back from failure
                Console.WriteLine("CAME BACK FROM THE DEAD, BRAINS..." + worker_url + "!!!");
            }
        }
    }
}
