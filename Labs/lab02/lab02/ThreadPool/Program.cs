using System;
using System.Threading;

delegate void ThrWork();

class ThrPool
{
    ThrWork[] buffer;   // The circular buffer
    Thread[] pool;  // The thread pool
    int bufIndex = 0;   // last available index of circular buffer
    Boolean end = false;

    public ThrPool(int thrNum, int bufSize)
    {
        buffer = new ThrWork[bufSize];
        InitializePool(thrNum);
    }

    public void InitializePool(int thrNum)
    {
        ThreadStart ts = new ThreadStart(Execute);
        pool = new Thread[thrNum];
        for (int i = 0; i < thrNum; i++)
        {
            pool[i] = new Thread(ts);
            pool[i].Start();
        }
    }

    public void AssyncInvoke(ThrWork action)
    {
        Monitor.Enter(buffer);
        if (bufIndex < buffer.Length)
        {
            buffer[bufIndex] = action;
            bufIndex++;
        }
        else
        {
            bufIndex = 0;
        }
        Monitor.Exit(buffer);
    }

    public void Execute()
    {
        while (!end)
        {
            Monitor.Enter(buffer);
            if (bufIndex > 0)
            {
                bufIndex--;
                buffer[bufIndex]();
            }
            Monitor.Exit(buffer);
        }
    }

    public void setEnd(Boolean end)
    {
        this.end = end;
    }
}


class A
{
    private int _id;

    public A(int id)
    {
        _id = id;
    }

    public void DoWorkA()
    {
        Console.WriteLine("A-{0}", _id);
    }
}


class B
{
    private int _id;

    public B(int id)
    {
        _id = id;
    }

    public void DoWorkB()
    {
        Console.WriteLine("B-{0}", _id);
    }
}


class Test
{
    public static void Main()
    {
        ThrPool tpool = new ThrPool(5, 10);

        for (int i = 0; i < 10; i++)
        {
            A a = new A(i);
            tpool.AssyncInvoke(new ThrWork(a.DoWorkA));

            B b = new B(i);
            tpool.AssyncInvoke(new ThrWork(b.DoWorkB));
        }

        tpool.setEnd(true);
        Console.ReadLine();
    }
}
