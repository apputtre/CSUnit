using System.Diagnostics;
using System.Reflection;

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
                AssertionFailed f = e.InnerException as AssertionFailed ?? throw new System.Exception();

                result.Log += $"Test {name} failed: {f.Message} (file {f.File}, line {f.Line})\n\t";
            }
            else
                throw e.InnerException ?? new System.Exception();
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
            StackFrame frame = trace.GetFrame(1) ?? throw new System.Exception();
            file = frame.GetFileName() ?? "n/a";
            line = frame.GetFileLineNumber();

            if (msg == "")
                throw new AssertionFailed(name, file, line);
            else
                throw new AssertionFailed(name, file, line, msg);
        }
    }
}