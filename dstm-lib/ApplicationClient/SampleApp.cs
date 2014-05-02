using System;
using PADI_DSTM;

class SampleApp {
    static void Main(string[] args) {
        bool res;

        PadiDstm.Init();

        res = PadiDstm.TxBegin();
        PadInt pi_a = PadiDstm.CreatePadInt(12);
        PadInt pi_b = PadiDstm.CreatePadInt(20);
        PadInt pi_c = PadiDstm.CreatePadInt(43);
        res = PadiDstm.TxCommit();

        res = PadiDstm.TxBegin();
        pi_a = PadiDstm.AccessPadInt(12);
        pi_b = PadiDstm.AccessPadInt(20);
        pi_c = PadiDstm.AccessPadInt(43);
        pi_a.Write(36);
        pi_b.Write(37);
        pi_c.Write(69);
        Console.WriteLine("a = " + pi_a.Read());
        Console.WriteLine("b = " + pi_b.Read());
        Console.WriteLine("c = " + pi_c.Read());
        PadiDstm.Status();
        // The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
        res = PadiDstm.Freeze("tcp://localhost:2001/Server");
        res = PadiDstm.Recover("tcp://localhost:2001/Server");
        res = PadiDstm.Fail("tcp://localhost:2002/Server");
        res = PadiDstm.TxCommit();
        Console.ReadLine();
    }
}
