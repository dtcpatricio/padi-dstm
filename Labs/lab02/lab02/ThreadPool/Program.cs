using System;
using System.Threading;

delegate void ThrWork();

class ThrPool
{
    ThrWork[] buffer;   // The circular buffer
    Thread[] pool;  // The thread pool
    int bufIndex = 0;   // last available index of circular buffer
    int thrIndex = 0;
    Boolean end = false;

    public ThrPool(int thrNum, int bufSize)
    {
        buffer = new ThrWork[bufSize];
        InitializePool(thrNum);
    }

    public void InitializePool(int thrNum)
    {
        ThreadStart ts;
        pool = new Thread[thrNum];
        for (int i = 0; i < thrNum; i++)
        {
            ts = new ThreadStart(Execute);
            pool[i] = new Thread(ts);
            pool[i].Start();
        }
    }

    public void AssyncInvoke(ThrWork action)
    {
        lock (this)
        {

            while (bufIndex == buffer.Length)
            {
                Monitor.Wait(this);
            }

            buffer[bufIndex] = action;
            bufIndex++;
            Monitor.PulseAll(this);

        }
    }

    public void Execute()
    {
        while (!end)
        {
            lock (this)
            {
                while (bufIndex == 0 || thrIndex == pool.Length)
                {
                    Monitor.Wait(this);
                }

                bufIndex--;
                ThrWork ex = (ThrWork)buffer[bufIndex];
                thrIndex++;
                ex();
                thrIndex--;

                Monitor.PulseAll(this);
            }
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

        //tpool.setEnd(true);
        Console.ReadLine();
    }
}
