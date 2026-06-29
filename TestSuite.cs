using System;
using System.Collections;
using System.Reflection;

class TestSuite
{
    ArrayList tests = new ArrayList();

    public TestSuite() {}

    public TestSuite(Type test)
    {
        foreach (MethodInfo method in test.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (method.Name.StartsWith("Test"))
            {
                object instance = Activator.CreateInstance(test, method.Name) ?? throw new Exception();

                Add((TestCase) instance);
            }
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