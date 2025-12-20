// <copyright file="TestRunner.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNUnit;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// Executes discovered tests.
/// </summary>
public static class TestRunner
{
    /// <summary>
    /// Runs all tests in given test classes.
    /// </summary>
    /// <param name="testClasses">Collection of types containing test methods.</param>
    /// <returns>
    /// Collection of results for all executed tests.
    /// </returns>
    public static IReadOnlyList<TestResult> Run(IEnumerable<Type> testClasses)
    {
        ArgumentNullException.ThrowIfNull(testClasses);

        var results = new ConcurrentBag<TestResult>();

        Parallel.ForEach(testClasses, testClass =>
        {
            RunTestsInClass(testClass, results);
        });

        return results.ToList();
    }

    private static void RunTestsInClass(Type testClass, ConcurrentBag<TestResult> results)
    {
        var beforeClass = GetMethods<BeforeClassAttribute>(testClass, true);
        var afterClass = GetMethods<AfterClassAttribute>(testClass, true);
        var before = GetMethods<BeforeAttribute>(testClass, false);
        var after = GetMethods<AfterAttribute>(testClass, false);
        var tests = GetMethods<TestAttribute>(testClass, false);

        try
        {
            InvokeStatic(beforeClass);

            foreach (var test in tests)
            {
                results.Add(RunSingleTest(testClass, test, before, after));
            }
        }
        catch (Exception ex)
        {
            foreach (var test in tests)
            {
                results.Add(new TestResult
                {
                    TestName = $"{testClass.FullName}.{test.Name}",
                    Status = TestStatus.Failed,
                    Message = "BeforeClass failed",
                    Exception = ex,
                });
            }
        }
        finally
        {
            InvokeStatic(afterClass);
        }
    }

    private static TestResult RunSingleTest(Type testClass, MethodInfo testMethod, IReadOnlyList<MethodInfo> before, IReadOnlyList<MethodInfo> after)
    {
        var testName = $"{testClass.FullName}.{testMethod.Name}";
        var ignore = testMethod.GetCustomAttribute<IgnoreAttribute>();
        var testAttr = testMethod.GetCustomAttribute<TestAttribute>();

        if (ignore != null)
        {
            return new TestResult
            {
                TestName = testName,
                Status = TestStatus.Ignored,
                Message = ignore.Reason,
                Duration = TimeSpan.Zero,
            };
        }

        if (!IsValidTestMethod(testMethod))
        {
            return new TestResult
            {
                TestName = testName,
                Status = TestStatus.Failed,
                Message = "Test method must be public void with no parameters",
            };
        }

        var stopwatch = Stopwatch.StartNew();
        object? instance = null;

        try
        {
            instance = Activator.CreateInstance(testClass) ?? throw new InvalidOperationException($"Failed to create instance of {testClass.FullName}");
            InvokeInstance(before, instance);
            testMethod.Invoke(instance, null);
            InvokeInstance(after, instance);

            stopwatch.Stop();

            if (testAttr?.Expected != null)
            {
                return Fail(testName, "Expected exception was not thrown", stopwatch.Elapsed);
            }

            return Pass(testName, stopwatch.Elapsed);
        }
        catch (TargetInvocationException ex)
        {
            stopwatch.Stop();
            InvokeAfterSafely(after, instance);

            var actual = ex.InnerException!;

            if (testAttr?.Expected != null &&
                testAttr.Expected.IsAssignableFrom(actual.GetType()))
            {
                return Pass(testName, stopwatch.Elapsed);
            }

            return Fail(testName, actual.Message, stopwatch.Elapsed, actual);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            InvokeAfterSafely(after, instance);

            return Fail(testName, ex.Message, stopwatch.Elapsed, ex);
        }
    }

    private static bool IsValidTestMethod(MethodInfo method)
    {
        return method.IsPublic &&
               !method.IsStatic &&
               method.ReturnType == typeof(void) &&
               method.GetParameters().Length == 0;
    }

    private static IReadOnlyList<MethodInfo> GetMethods<T>(Type type, bool isStatic)
        where T : Attribute => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                               (isStatic ? BindingFlags.Static : BindingFlags.Instance))
            .Where(m => m.GetCustomAttribute<T>() != null)
            .ToList();

    private static void InvokeStatic(IEnumerable<MethodInfo> methods)
    {
        foreach (var method in methods)
        {
            if (!method.IsStatic)
            {
                throw new InvalidOperationException("BeforeClass/AfterClass must be static");
            }

            method.Invoke(null, null);
        }
    }

    private static void InvokeInstance(IEnumerable<MethodInfo> methods, object instance)
    {
        foreach (var method in methods)
        {
            method.Invoke(instance, null);
        }
    }

    private static void InvokeAfterSafely(IEnumerable<MethodInfo> methods, object? instance)
    {
        if (instance == null)
        {
            return;
        }

        try
        {
            InvokeInstance(methods, instance);
        }
        catch
        {
            // ignored
        }
    }

    private static TestResult Pass(string name, TimeSpan time)
    {
        return new TestResult
        {
            TestName = name,
            Status = TestStatus.Passed,
            Duration = time,
        };
    }

    private static TestResult Fail(string name, string message, TimeSpan time = default, Exception? exception = null)
    {
        return new TestResult
        {
            TestName = name,
            Status = TestStatus.Failed,
            Duration = time,
            Message = message,
            Exception = exception,
        };
    }
}
