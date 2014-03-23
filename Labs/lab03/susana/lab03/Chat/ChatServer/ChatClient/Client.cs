using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ChatClient
{
    static class Client
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void InitializeNewThread()
        {
            //Initialize a new Thread of name myThread to call Application.Run() on a new instance of ViewSecond
            Thread myThread = new Thread((ThreadStart)delegate { Application.Run(new Form1()); });
            //myThread.TrySetApartmentState(ApartmentState.STA); //If you receive errors, comment this out; use this when doing interop with STA COM objects.
            myThread.Start(); //Start the thread; Run the form
        }
    }
}
