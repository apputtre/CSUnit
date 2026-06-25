using System;
using System.Reflection;
using System.Collections;

class Program
{
    static void Main(string[] args)
    {
        TestSuite suite = new TestSuite();

        suite.add(new TestCaseTest("testTemplateMethod"));
        suite.add(new TestCaseTest("testResult"));
        suite.add(new TestCaseTest("testFailedResult"));
        suite.add(new TestCaseTest("testFailedResultFormatting"));
        suite.add(new TestCaseTest("testFailedSetUp"));

        TestResult result = new TestResult();
        suite.run(result);
        Console.WriteLine(result.summary());
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

    public void Run(TestResult result)
    {
        result.testStarted();
        try
        {
            setUp();
            GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, this, null);
        }
        catch
        {
            result.testFailed();
        }
        finally
        {
            tearDown();
        }
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

class TestSuite
{
    ArrayList tests = new ArrayList();

    public void add(TestCase test)
    {
        tests.Add(test);
    }

    public void run(TestResult result)
    {
        foreach (TestCase test in tests)
            test.Run(result);
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

class BrokenSetUp : WasRun
{
    public BrokenSetUp(string name) : base(name) {}

    protected override void setUp()
    {
        throw new Exception();
    }
}

class TestCaseTest : TestCase
{
    public TestCaseTest(string name) : base(name){}

    public void testTemplateMethod()
    {
        WasRun test = new WasRun("testMethod");
        TestResult result = new TestResult();
        test.Run(result);
        Assert(test.log == "setUp testMethod tearDown ");
    }

    public void testResult()
    {
        WasRun test = new WasRun("testMethod");
        TestResult result = new TestResult();
        test.Run(result);
        Assert(result.summary() == "1 run, 0 failed");
    }

    public void testFailedResult()
    {
        WasRun test = new WasRun("testBrokenMethod");
        TestResult result = new TestResult();
        test.Run(result);
        Assert(result.summary() == "1 run, 1 failed");
    }

    public void testFailedResultFormatting()
    {
        TestResult result = new TestResult();
        result.testStarted();
        result.testFailed();
        Assert(result.summary() == "1 run, 1 failed");
    }

    public void testFailedSetUp()
    {
        BrokenSetUp test = new BrokenSetUp("testMethod");
        TestResult result = new TestResult();
        test.Run(result);
        Assert(result.summary() == "1 run, 1 failed");
    }

    public void testSuite()
    {
        TestSuite suite = new TestSuite();
        suite.add(new WasRun("testMethod"));
        suite.add(new WasRun("testBrokenMethod"));
        TestResult result = new TestResult();
        suite.run(result);
        Assert(result.summary() == "2 run, 0 failed");
    }
}