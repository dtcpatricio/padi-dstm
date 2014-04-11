using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Padi_dstm;

namespace ApplicationClient
{
    class Program
    {
        static void Main(string[] args)
        {
            if (PadiDstm.Init())
            {
                PadiDstm.TxBegin();
                PadInt padint = PadiDstm.CreatePadInt(12);
                int val = padint.Read();

                Console.WriteLine(val);
                Console.ReadLine();
            }
        }
    }
}
