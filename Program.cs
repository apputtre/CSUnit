using System;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        TestCaseTest test_1 = new TestCaseTest("testTemplateMethod");
        test_1.Run();

        TestCaseTest test_2 = new TestCaseTest("testResult");
        test_2.Run();

        //TestCaseTest test_3 = new TestCaseTest("testFailedResult");
        //test_3.Run();

        TestCaseTest test_4 = new TestCaseTest("testFailedResultFormatting");
        test_4.Run();
    }
}

class TestResult
{
    private int runCount = 0;
    private int errorCount = 0;

    public void testStarted()
    {
        ++runCount;
    }

    public void testFailed()
    {
        ++errorCount;
    }

    public string summary()
    {
        return $"{runCount} run, {errorCount} failed";
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

    public TestResult Run()
    {
        TestResult result = new TestResult();

        result.testStarted();
        setUp();
        GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, this, null);
        tearDown();

        return result;
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

    public void testBrokenMethod()
    {
        throw new Exception();
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

    public void testResult()
    {
        WasRun test = new WasRun("testMethod");
        TestResult result = test.Run();
        Assert(result.summary() == "1 run, 0 failed");
    }

    public void testFailedResult()
    {
        WasRun test = new WasRun("testBrokenMethod");
        TestResult result = test.Run();
        Assert(result.summary() == "1 run, 1 failed");
    }

    public void testFailedResultFormatting()
    {
        TestResult result = new TestResult();
        result.testStarted();
        result.testFailed();
        Assert(result.summary() == "1 run, 1 failed");
    }
}