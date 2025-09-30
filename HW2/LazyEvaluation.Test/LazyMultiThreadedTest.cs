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
    public void LazyMultiThreaded_Get_SimpleString_ConcurrentCalls_SingleSupplierCall()
    {
        var callCount = 0;

        var supplier = new Func<string>(Supplier);
        var lazy = this.CreateLazy(supplier);
        const int taskCount = 10;
        var tasks = new Task[taskCount];

        for (var i = 0; i < taskCount; i++)
        {
            tasks[i] = Task.Run(() => lazy.Get());
        }

        Task.WhenAll(tasks).Wait();

        Assert.That(callCount, Is.EqualTo(1));
        return;

        string Supplier()
        {
            callCount++;
            return "test";
        }
    }

    /// <summary>
    /// Verifies that LazyMultiThreaded handles multiple concurrent calls without race conditions.
    /// </summary>
    [Test]
    public void LazyMultiThreaded_Get_SimpleString_MultipleConcurrentCalls_NoRaceConditions()
    {
        var callCount = 0;

        var supplier = new Func<string>(Supplier);
        var lazy = this.CreateLazy(supplier);
        const int taskCount = 100;
        var tasks = new Task<string>[taskCount];

        for (var i = 0; i < taskCount; i++)
        {
            tasks[i] = Task.Run(() => lazy.Get())!;
        }

        var results = Task.WhenAll(tasks).Result;

        Assert.That(callCount, Is.EqualTo(1));
        foreach (var result in results)
        {
            Assert.That(result, Is.EqualTo("test"));
        }

        return;

        string Supplier()
        {
            callCount++;
            return "test";
        }
    }

    /// <inheritdoc/>
    protected override ILazy<T> CreateLazy<T>(Func<T?> supplier)
        where T : default
    {
        return new LazyMultiThreaded<T>(supplier!);
    }
}