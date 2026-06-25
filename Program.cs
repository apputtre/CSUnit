using System;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        WasRun test = new WasRun("testMethod");
        Console.WriteLine(test.wasRun);
        test.Run();
        Console.WriteLine(test.wasRun);
    }
}


class WasRun
{
    public bool wasRun;

    private string name;

    public WasRun(string name)
    {
        wasRun = false;
        this.name = name;
    }

    public void Run()
    {
        typeof(WasRun).InvokeMember(name, BindingFlags.InvokeMethod, null, this, null);
        testMethod();
    }

    public void testMethod()
    {
        wasRun = true;
    }
}