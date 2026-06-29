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