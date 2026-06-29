using System;

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