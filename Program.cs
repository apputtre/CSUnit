using System;

class Program
{
    static void Main(string[] args)
    {
        WasRun test = new WasRun("testMethod");
        Console.WriteLine(test.wasRun);
        test.testMethod();
        Console.WriteLine(test.wasRun);
    }
}


class WasRun
{
    public bool wasRun;

    public WasRun(string name)
    {
        wasRun = false;
    }

    public void testMethod()
    {
        wasRun = true;
    }
}