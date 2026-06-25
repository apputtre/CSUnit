using System;
using System.Reflection;
using System.Collections;

class Program
{
    static void Main(string[] args)
    {
        TestSuite suite = new TestSuite(typeof(TestCaseTest));

        TestResult result = new TestResult();
        suite.Run(result);
        Console.WriteLine(result.Summary());
    }
}

class TestResult
{
    public int RunCount {get; private set;} = 0;
    public int ErrorCount {get; private set;} = 0;

    public void TestStarted()
    {
        ++RunCount;
    }

    public void TestFailed()
    {
        ++ErrorCount;
    }

    public string Summary()
    {
        return $"{RunCount} run, {ErrorCount} failed";
    }
}

class TestCase
{
    protected string name;

    public TestCase(string name)
    {
        this.name = name;
    }

    protected virtual void SetUp() {}

    protected virtual void TearDown() {}

    public void Run(TestResult result)
    {
        result.TestStarted();
        try
        {
            SetUp();
            GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, this, null);
        }
        catch
        {
            result.TestFailed();
        }
        finally
        {
            TearDown();
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

    public TestSuite() {}

    public TestSuite(Type test)
    {
        foreach (MethodInfo method in test.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (method.Name.StartsWith("Test"))
                Add((TestCase) Activator.CreateInstance(test, method.Name));
        }
    }

    public void Add(TestCase test)
    {
        tests.Add(test);
    }

    public void Run(TestResult result)
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
        test.Run(result);
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
        test.Run(result);
        Assert(result.Summary() == "1 run, 1 failed");
        Assert(test.log == "tearDown ");
    }

    public void TestSuite()
    {
        TestSuite suite = new TestSuite();
        suite.Add(new WasRun("TestMethod"));
        suite.Add(new WasRun("TestBrokenMethod"));
        suite.Run(result);
        Assert(result.Summary() == $"2 run, 1 failed");
    }
}