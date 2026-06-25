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

class TestCase
{
    protected string name;

    public TestCase(string name)
    {
        this.name = name;
    }

    public void Run()
    {
        GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, this, null);
    }
}

class WasRun : TestCase
{
    public bool wasRun;

    public WasRun(string name) : base(name)
    {
        wasRun = false;
    }

    public void testMethod()
    {
        wasRun = true;
    }
}