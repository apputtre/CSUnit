using System;
using System.Reflection;
using System.Collections;
using System.Diagnostics;

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

class AssertionFailed : Exception
{
    public string Test {get; private set;}
    public string File {get; private set;}
    public int Line {get; private set;}

    public AssertionFailed(string test, string file, int line) : base("Assertion failed")
    {
        this.Test = test;
        this.File = file;
        this.Line = line;
    }

    public AssertionFailed(string test, string file, int line, string msg) : base($"Assertion failed: {msg}")
    {
        this.Test = test;
        this.File = file;
        this.Line = line;
    }
}

class TestResult
{
    public int RunCount {get; private set;} = 0;
    public int ErrorCount {get; private set;} = 0;
    public string Log {get; set;} = "";

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
        string summary = GetHeader();

        if (Log != "")
            summary += "\n\t" + Log;

        return summary;
    }

    private string GetHeader()
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
        }
        catch
        {
            result.TestFailed();
            TearDown();

            throw;
        }

        try
        {
            GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, this, null);
        }
        catch (TargetInvocationException e)
        {
            result.TestFailed();

            if (e.InnerException is AssertionFailed)
            {
                AssertionFailed f = e.InnerException as AssertionFailed;

                result.Log += $"Test {name} failed: {f.Message} (file {f.File}, line {f.Line})\n\t";
            }
            else
                throw e.InnerException;
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
            string file;
            int line;

            StackTrace trace = new StackTrace(true);
            StackFrame frame = trace.GetFrame(1);
            file = frame.GetFileName();
            line = frame.GetFileLineNumber();

            if (msg == "")
                throw new AssertionFailed(name, file, line);
            else
                throw new AssertionFailed(name, file, line, msg);
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