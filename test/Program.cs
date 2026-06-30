using System;
using System.Reflection;
using System.Collections.Generic;

class Test 
{
    static void Main(string[] args)
    {
        TestSuite suite = new TestSuite();
        suite.Add(new TestCaseTest());

        TestResult result = new TestResult();
        suite.Run(result);
        Console.WriteLine(result.Summary());
    }
}

class WasRun : TestCase
{
    public string log;

    public WasRun()
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
    public BrokenSetUp() : base() {}

    protected override void SetUp()
    {
        throw new Exception();
    }
}

class TestCaseTest : TestCase
{
    private TestResult result;

    public TestCaseTest() : base()
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
        WasRun test = new WasRun();
        test.Run(result, "TestMethod");
        Assert(test.log == "setUp testMethod tearDown ");
    }

    public void TestResult()
    {
        WasRun test = new WasRun();
        test.Run(result, "TestMethod");
        Assert(result.Summary() == "1 run, 0 failed");
    }

    public void TestFailedResult()
    {
        WasRun test = new WasRun();

        bool exceptionThrown = false;

        try
        {
            test.Run(result, "TestBrokenMethod");
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
        BrokenSetUp test = new BrokenSetUp();

        bool exceptionThrown = false;

        try
        {
            test.Run(result, "TestMethod");
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

        suite.Add(new WasRun(), new List<string>() { "TestMethod", "TestBrokenMethod" });

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