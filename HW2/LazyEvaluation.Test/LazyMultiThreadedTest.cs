// <copyright file="LazyMultiThreadedTest.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace LazyEvaluation.Test;

/// <summary>
/// Unit tests for LazyMultiThreaded implementation.
/// </summary>
public class LazyMultiThreadedTest : LazyEvaluationTest
{
    /// <summary>
    /// Verifies that the supplier is called exactly once during concurrent Get() calls in LazyMultiThreaded.
    /// </summary>
    [Test]
    public void LazyMultiThreaded_Get_String_SingleSupplierCall()
    {
        var callCount = 0;
        var lazy = this.CreateLazy(() =>
        {
        Interlocked.Increment(ref callCount);
        return "test";
        });

        const int threadsCount = 100;
        var threads = new Thread[threadsCount];
        var results = new string[threadsCount];

        for (var i = 0; i < threadsCount; i++)
        {
            var index = i;
            threads[i] = new Thread(() =>
            {
                results[index] = lazy.Get() ?? string.Empty;
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.Multiple(() =>
            {
                Assert.That(results, Has.All.EqualTo("test"));
                Assert.That(callCount, Is.EqualTo(1));
            });
    }

    /// <summary>
    /// Verifies that LazyMultiThreaded handles multiple concurrent calls without race conditions.
    /// </summary>
    [Test]
    public void LazyMultiThreaded_Get_String_NoRaceConditions()
    {
        var callCount = 0;
        var lazy = this.CreateLazy(() =>
        {
            Interlocked.Increment(ref callCount);
            return "test";
        });

        const int threadsCount = 100;
        var threads = new Thread[threadsCount];
        var results = new string?[threadsCount];

        for (var i = 0; i < threadsCount; i++)
        {
            var index = i;
            threads[i] = new Thread(() =>
            {
                results[index] = lazy.Get()!;
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.Multiple(() =>
        {
            Assert.That(results, Has.All.EqualTo("test"));
            Assert.That(callCount, Is.EqualTo(1));
        });
    }

    /// <inheritdoc/>
    protected override ILazy<T> CreateLazy<T>(Func<T> supplier)
    {
        return new LazySingleThreaded<T>(supplier);
    }
}
