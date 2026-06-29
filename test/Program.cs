using System;
using System.Reflection;
using System.Collections;

class Test 
{
    static void Main(string[] args)
    {
        TestSuite suite = new TestSuite(typeof(TestCaseTest));

        TestResult result = new TestResult();
        suite.Run(result);
        Console.WriteLine(result.Summary());
    }
}

class WasRun : TestCase
{
    public string log;

    public WasRun(string name) : base(name)
    {
        log = "";
    }

    public void TestMethod()
    {
        log += "testMethod ";
    }

    public void TestBrokenMethod()
    {
        throw new Exception();
    }

    protected override void SetUp()
    {
        log += "setUp ";
    }

    protected override void TearDown()
    {
        log += "tearDown ";
    }
}

class BrokenSetUp : WasRun
{
    public BrokenSetUp(string name) : base(name) {}

    protected override void SetUp()
    {
        throw new Exception();
    }
}

class TestCaseTest : TestCase
{
    private TestResult result;

    public TestCaseTest(string name) : base(name)
    {
        result = new TestResult();
    }

    protected override void SetUp()
    {
        result = new TestResult();

        base.SetUp();
    }

    public void TestTemplateMethod()
    {
        WasRun test = new WasRun("TestMethod");
        test.Run(result);
        Assert(test.log == "setUp testMethod tearDown ");
    }

    public void TestResult()
    {
        WasRun test = new WasRun("TestMethod");
        test.Run(result);
        Assert(result.Summary() == "1 run, 0 failed");
    }

    public void TestFailedResult()
    {
        WasRun test = new WasRun("TestBrokenMethod");

        bool exceptionThrown = false;

        try
        {
            test.Run(result);
        }
        catch
        {
            exceptionThrown = true;
        }

        Assert(exceptionThrown == true);
        Assert(result.Summary() == "1 run, 1 failed");
    }

    public void TestFailedResultFormatting()
    {
        result.TestStarted();
        result.TestFailed();
        Assert(result.Summary() == "1 run, 1 failed");
    }

    public void TestFailedSetUp()
    {
        BrokenSetUp test = new BrokenSetUp("TestMethod");

        bool exceptionThrown = false;

        try
        {
            test.Run(result);
        }
        catch
        {
            exceptionThrown = true;
        }

        Assert(exceptionThrown == true);
        Assert(result.Summary() == "1 run, 1 failed");
        Assert(test.log == "tearDown ");
    }

    public void TestSuite()
    {
        TestSuite suite = new TestSuite();
        suite.Add(new WasRun("TestMethod"));
        suite.Add(new WasRun("TestBrokenMethod"));

        bool exceptionThrown = false;

        try
        {
            suite.Run(result);
        }
        catch
        {
            exceptionThrown = true;
        }

        Assert(exceptionThrown == true);
        Assert(result.Summary() == $"2 run, 1 failed");
    }
}