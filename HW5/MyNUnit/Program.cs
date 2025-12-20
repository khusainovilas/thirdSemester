// <copyright file="Program.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

using MyNUnit;

if (args.Length != 1)
{
    PrintUsage();
    return;
}

try
{
    var testClasses = TestDiscovery.Discover(args[0]);

    if (testClasses.Count == 0)
    {
        Console.WriteLine("No tests found.");
        return;
    }

    var results = TestRunner.Run(testClasses);
    ConsoleReporter.Print(results);
}
catch (Exception ex)
{
    Console.WriteLine("[ERROR]");
    Console.WriteLine(ex.Message);
}

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  MyNUnit <path>");
    Console.WriteLine();
    Console.WriteLine("Arguments:");
    Console.WriteLine("  <path>  Path to directory or .dll file containing tests.");
}
