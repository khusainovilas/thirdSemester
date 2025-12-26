// <copyright file="ConsoleReporter.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNUnit;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Prints test results to console.
/// </summary>
public static class ConsoleReporter
{
    /// <summary>
    /// Prints test execution report.
    /// </summary>
    /// <param name="results">A collection of objects to print.</param>
    public static void Print(IEnumerable<TestResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var list = results.ToList();

        foreach (var result in list.OrderBy(r => r.TestName))
        {
            PrintSingle(result);
        }

        PrintSummary(list);
    }

    private static void PrintSingle(TestResult result)
    {
        switch (result.Status)
        {
            case TestStatus.Passed:
                Console.WriteLine(
                    $"[PASS] {result.TestName} ({Format(result.Duration)})");
                break;

            case TestStatus.Failed:
                Console.WriteLine(
                    $"[FAIL] {result.TestName} ({Format(result.Duration)})");

                if (!string.IsNullOrWhiteSpace(result.Message))
                {
                    Console.WriteLine($"       {result.Message}");
                }

                if (result.Exception != null)
                {
                    Console.WriteLine(
                        $"       {result.Exception.GetType().Name}: {result.Exception.Message}");
                }

                break;

            case TestStatus.Ignored:
                Console.WriteLine($"[IGNORED] {result.TestName}");
                Console.WriteLine($"          Reason: {result.Message}");
                break;
        }
    }

    private static void PrintSummary(IReadOnlyCollection<TestResult> results)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 40));

        Console.WriteLine($"Total:   {results.Count}");
        Console.WriteLine($"Passed:  {results.Count(r => r.Status == TestStatus.Passed)}");
        Console.WriteLine($"Failed:  {results.Count(r => r.Status == TestStatus.Failed)}");
        Console.WriteLine($"Ignored: {results.Count(r => r.Status == TestStatus.Ignored)}");

        var totalTime = TimeSpan.FromTicks(results.Sum(r => r.Duration.Ticks));
        Console.WriteLine($"Time:    {Format(totalTime)}");

        Console.WriteLine(new string('=', 40));
    }

    private static string Format(TimeSpan time)
    {
        return time.TotalMilliseconds < 1
            ? "<1 ms"
            : $"{time.TotalMilliseconds:F0} ms";
    }
}
