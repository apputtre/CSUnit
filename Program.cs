using System;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        TestCaseTest test = new TestCaseTest("testRunning");

        test.Run();
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

class TestCaseTest : TestCase
{
    public TestCaseTest(string name) : base(name){}

    public void testRunning()
    {
        WasRun test = new WasRun("testMethod");

        if (test.wasRun)
            throw new Exception("wasRun was true when it should be false");

        test.Run();

        if (!test.wasRun)
            throw new Exception("wasRun was false when it should be true");
    }
}