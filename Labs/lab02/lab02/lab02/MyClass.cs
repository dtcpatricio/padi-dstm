// compose.cs
using System;

delegate void MyDelegate(string s);

class MyClass
{
    public static void Hello(string s)
    {
        Console.WriteLine("  Hello, " + s);
    }

    public static void Goodbye(string s)
    {
        Console.WriteLine("  Goodbye, " + s);
    }

    public void Delegate(MyDelegate md)
    {
        md("Susana");
    }

    public static void Main()
    {
        MyDelegate a, b, c, d, s;

        // Create the delegate object a that references
        // the method Hello:
        a = new MyDelegate(Hello);
        // Create the delegate object b that references
        // the method Goodbye:
        b = new MyDelegate(Goodbye);
        // The two delegates, a and b, are composed to form c:
        c = a + b;
        // Remove a from the composed delegate, leaving d,
        // which calls only the method Goodbye:
        d = c - a;

        MyClass mc = new MyClass();
        s = new MyDelegate(Hello);

        Console.ReadLine();

        Console.WriteLine("Invoking delegate a:");
        a("A");
        Console.WriteLine("Invoking delegate b:");
        b("B");
        Console.WriteLine("Invoking delegate c:");
        c("C");
        Console.WriteLine("Invoking delegate d:");
        d("D");

        mc.Delegate(s);

        Console.ReadLine();
    }
}
