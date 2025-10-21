// <copyright file="MyThreadPoolTest.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyThreadPool.Test;

using System.Diagnostics;

/// <summary>
/// Unit tests for MyThreadPool.
/// </summary>
public class MyThreadPoolTest
{
    /// <summary>
    /// Tests that submitting a simple task returns the correct result and completes.
    /// </summary>
    [Test]
    public void MyThreadPool_Submit_ReturnsCorrectResult()
    {
        var pool = new MyThreadPool(1);
        var task = pool.Submit(() => 42);
        Assert.Multiple(() =>
        {
            Assert.That(task.Result, Is.EqualTo(42));
            Assert.That(task.IsCompleted, Is.True);
        });
    }

    /// <summary>
    /// Tests that a continuation task executes correctly with the transformed result.
    /// </summary>
    [Test]
    public void IMyTask_ContinueWith_ExecutesCorrectly()
    {
        var pool = new MyThreadPool(1);
        var task = pool.Submit(() => 2 * 2);
        var continuation = task.ContinueWith(x => x.ToString());
        Assert.Multiple(() =>
        {
            Assert.That(continuation.Result, Is.EqualTo("4"));
            Assert.That(continuation.IsCompleted, Is.True);
        });
    }

    /// <summary>
    /// Tests that an exception in a task is wrapped in AggregateException and the task completes.
    /// </summary>
    [Test]
    public void IMyTask_Result_ThrowsAggregateExceptionOnError()
    {
        var pool = new MyThreadPool(1);
        var task = pool.Submit<int>(() => throw new InvalidOperationException("Test error"));
        var exception = Assert.Throws<AggregateException>(() => _ = task.Result);
        Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
        Assert.Multiple(() =>
        {
            Assert.That(exception.InnerException.Message, Is.EqualTo("Test error"));
            Assert.That(task.IsCompleted, Is.True);
        });
    }

    /// <summary>
    /// Tests that multiple continuations for a single task execute correctly with different transformations.
    /// </summary>
    [Test]
    public void IMyTask_ContinueWith_SupportsMultipleContinuations()
    {
        var pool = new MyThreadPool(1);
        var task = pool.Submit(() => 5);
        var cont1 = task.ContinueWith(x => x + 1);
        var cont2 = task.ContinueWith(x => x * 2);
        Assert.Multiple(() =>
        {
            Assert.That(cont1.Result, Is.EqualTo(6));
            Assert.That(cont2.Result, Is.EqualTo(10));
            Assert.That(cont1.IsCompleted, Is.True);
            Assert.That(cont2.IsCompleted, Is.True);
        });
    }

    /// <summary>
    /// Tests that shutdown prevents new tasks and allows existing tasks to complete.
    /// </summary>
    [Test]
    public void MyThreadPool_Shutdown_PreventsNewTasksAndCompletesExisting()
    {
        var pool = new MyThreadPool(1);
        var task = pool.Submit(() =>
        {
            Thread.Sleep(100);
            return 1;
        });
        pool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => pool.Submit(() => 2));
        Assert.Multiple(() =>
        {
            Assert.That(task.Result, Is.EqualTo(1));
            Assert.That(task.IsCompleted, Is.True);
        });
    }

    /// <summary>
    /// Tests that the thread pool uses at least n threads by checking parallel execution time.
    /// </summary>
    [Test]
    public void MyThreadPool_ThreadCount_UsesAtLeastNThreads()
    {
        const int n = 4;
        var pool = new MyThreadPool(n);
        var tasks = new List<IMyTask<int>>();
        var stopwatch = Stopwatch.StartNew();
        for (var i = 0; i < n; i++)
        {
            tasks.Add(pool.Submit(() =>
            {
                Thread.Sleep(1000);
                return 1;
            }));
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EqualTo(1));
        }

        stopwatch.Stop();

        // The work of 4 threads will take approximately 1 second.
        Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(1.5));
    }
}