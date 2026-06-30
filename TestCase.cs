using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

class TestCase
{
    private string curr_test = "";

    protected virtual void SetUp() {}

    protected virtual void TearDown() {}

    public void Run(TestResult result, string testName)
    {
        curr_test = testName;
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
            GetType().InvokeMember(testName, BindingFlags.InvokeMethod, null, this, null);
        }
        catch (TargetInvocationException e)
        {
            result.TestFailed();

            if (e.InnerException is AssertionFailed)
            {
                AssertionFailed f = e.InnerException as AssertionFailed ?? throw new System.Exception();

                result.Log += $"Test {testName} failed: {f.Message} (file {f.File}, line {f.Line})\n\t";
            }
            else
                throw e.InnerException ?? new System.Exception();
        }
        finally
        {
            TearDown();
        }
    }

    public void Run(TestResult result)
    {
        MethodInfo[] tests = GetTests();

        foreach (MethodInfo m in tests)
            Run(result, m.Name);
    }

    protected void Assert(bool condition, string msg = "")
    {
        if (!condition)
        {
            string file;
            int line;

            StackTrace trace = new StackTrace(true);
            StackFrame frame = trace.GetFrame(1) ?? throw new System.Exception();
            file = frame.GetFileName() ?? "n/a";
            line = frame.GetFileLineNumber();

            if (msg == "")
                throw new AssertionFailed(curr_test, file, line);
            else
                throw new AssertionFailed(curr_test, file, line, msg);
        }
    }

    protected MethodInfo[] GetTests()
    {
        List<MethodInfo> ret = new List<MethodInfo>();

        MethodInfo[] tests = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);

        foreach (MethodInfo m in tests)
            if (m.Name.StartsWith("Test"))
                ret.Add(m);

        return ret.ToArray();
    }
}