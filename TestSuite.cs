using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

class TestSuite
{
    List<TestCase> tests = new List<TestCase>();
    List<List<string>> testNames = new List<List<string>>();

    public TestSuite() {}

    public void Add(TestCase test)
    {
        tests.Add(test);
        testNames.Add(new List<string>());
    }

    public void Add(TestCase test, string name)
    {
        tests.Add(test);
        testNames.Add(new List<string>() { name });
    }

    public void Add(TestCase test, List<string> names)
    {
        tests.Add(test);
        testNames.Add(names);
    }

    public void Run(TestResult result)
    {
        for (int i = 0; i < tests.Count; ++i)
        {
            if (testNames[i].Count == 0)
                tests[i].Run(result);
            else
            {
                foreach (string name in testNames[i])
                    tests[i].Run(result, name);
            }
        }
    }
}