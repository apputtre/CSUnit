using System;

class Program
{
    static void Main(string[] args)
    {
        WasRun test = new WasRun("testMethod");
        Console.WriteLine(test.wasRun);
        test.Run();
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

    public void Run()
    {
        testMethod();
    }

    public void testMethod()
    {
        wasRun = true;
    }
}