using System;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        TestCaseTest test_running = new TestCaseTest("testRunning");
        test_running.Run();

        TestCaseTest test_setUp = new TestCaseTest("testSetUp");
        test_setUp.Run();
    }
}

class TestCase
{
    protected string name;

    public TestCase(string name)
    {
        this.name = name;
    }

    public virtual void setUp() {}

    public void Run()
    {
        setUp();
        GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, this, null);
    }

    protected void Assert(bool condition, string msg = "")
    {
        if (!condition)
        {
            if (msg == "")
                throw new Exception("Assertion failed");
            else
                throw new Exception("Assertion failed: " + msg);
        }
    }
}

class WasRun : TestCase
{
    public bool wasRun;
    public bool wasSetUp;

    public WasRun(string name) : base(name) {}

    public void testMethod()
    {
        wasRun = true;
    }

    public override void setUp()
    {
        wasRun = false;
        wasSetUp = true;
    }
}

class TestCaseTest : TestCase
{
    public TestCaseTest(string name) : base(name){}

    WasRun? test;

    public override void setUp()
    {
        test = new WasRun("testMethod");
    }

    public void testRunning()
    {
        if (test != null)
            test.Run();
        else
            throw new Exception();

        Assert(test.wasRun, "wasRun was false when it should be true");
    }

    public void testSetUp()
    {
        if (test != null)
            test.Run();
        else
            throw new Exception();

        Assert(test.wasSetUp, "setUp was not run");
    }
}