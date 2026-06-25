using System;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        TestCaseTest test = new TestCaseTest("testTemplateMethod");
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

    protected virtual void setUp() {}

    protected virtual void tearDown() {}

    public void Run()
    {
        setUp();
        GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, this, null);
        tearDown();
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
    public string log;

    public WasRun(string name) : base(name)
    {
        log = "";
    }

    public void testMethod()
    {
        log += "testMethod ";
    }

    protected override void setUp()
    {
        log += "setUp ";
    }

    protected override void tearDown()
    {
        log += "tearDown ";
    }
}

class TestCaseTest : TestCase
{
    public TestCaseTest(string name) : base(name){}

    public void testTemplateMethod()
    {
        WasRun test = new WasRun("testMethod");
        test.Run();
        Assert(test.log == "setUp testMethod tearDown ");
    }
}