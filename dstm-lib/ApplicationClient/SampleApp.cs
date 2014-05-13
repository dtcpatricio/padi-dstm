﻿using System;
using PADI_DSTM;

class SampleApp {
  
    static void Main(string[] args) {
        bool res;

        PadiDstm.Init();
        res = PadiDstm.TxBegin();
        Console.WriteLine("Transaction = " + res);
        PadInt pi_a = PadiDstm.CreatePadInt(12);
        PadInt pi_b = PadiDstm.CreatePadInt(20);
        PadInt pi_c = PadiDstm.CreatePadInt(43);

        Console.WriteLine("Press <Enter> and kill server with UID = 20");
        Console.ReadLine();

        pi_b = PadiDstm.AccessPadInt(20);

        res = PadiDstm.TxCommit();

        Console.WriteLine("Transaction committed = " + res);

        Console.WriteLine("Press <Enter> to start new transaction");
        Console.ReadLine();

        res = PadiDstm.TxBegin();
        Console.WriteLine("Transaction = " + res);
        pi_a = PadiDstm.AccessPadInt(12);
        pi_b = PadiDstm.AccessPadInt(20); // NOW IT SHOULD WORK BECAUSE OF CHAIN REPLICATION
        pi_c = PadiDstm.AccessPadInt(43);
        pi_a.Write(36);
        pi_b.Write(37);
        pi_c.Write(69);

        pi_c.Write(70);
        
        Console.WriteLine("a = " + pi_a.Read());
        Console.WriteLine("b = " + pi_b.Read());
        Console.WriteLine("c = " + pi_c.Read());
        PadiDstm.Status();
        // The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
        res = PadiDstm.Freeze("tcp://localhost:2001/Server");
        res = PadiDstm.Recover("tcp://localhost:2001/Server");
        res = PadiDstm.Fail("tcp://localhost:2002/Server");

        Console.ReadLine();
        res = PadiDstm.TxCommit();
        Console.WriteLine("Transaction committed = " + res);
        Console.ReadLine();
    }
   
}
