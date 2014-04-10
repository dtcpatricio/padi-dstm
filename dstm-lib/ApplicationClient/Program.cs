﻿using System;
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
                PadInt padint = PadiDstm.AccessPadInt(12);
                int val = padint.Read();
                System.Console.WriteLine(val);
                System.Console.ReadLine();
            }
        }
    }
}
