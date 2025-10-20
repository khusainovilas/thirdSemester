// <copyright file="MyThreadPool.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Main thread pool class.
/// </summary>
public class MyThreadPool
{
    private readonly ManualResetEvent shutdownEvent;
    private readonly Thread[] threads;
    private readonly Queue<Action> taskQueue;
    private readonly object queueLock;
    private int threadCount;
    private volatile bool isShutdown;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="threadCount">The number of threads in the pool. Must be positive.</param>
    public MyThreadPool(int threadCount)
    {
        if (threadCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(threadCount), "Thread count must be positive.");
        }

        this.threadCount = threadCount;
        this.threads = new Thread[threadCount];
        this.taskQueue = new Queue<Action>();
        this.queueLock = new object();
        this.isShutdown = false;
        this.shutdownEvent = new ManualResetEvent(false);

        for (var i = 0; i < threadCount; i++)
        {
            this.threads[i] = new Thread(this.ThreadProc)
            {
                IsBackground = true,
                Name = $"ThreadPoolThread-{i}",
            };
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Submits a task to the thread pool for execution.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
    /// <param name="func">The function to execute to produce the result.</param>
    /// <returns>An <see cref="IMyTask{TResult}"/> representing the task.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        if (this.isShutdown)
        {
            throw new InvalidOperationException("Cannot submit tasks to a shutdown thread pool.");
        }

        var task = new MyTask<TResult>(this, func);
        this.QueueTask(task.GetExecuteAction() ?? throw new InvalidOperationException());
        return task;
    }

    /// <summary>
    /// Initiates a collaborative shutdown, preventing new tasks from being queued and allowing existing tasks to complete.
    /// Blocks until all threads have finished.
    /// </summary>
    public void Shutdown()
    {
        lock (this.queueLock)
        {
            if (this.isShutdown)
            {
                return;
            }

            this.isShutdown = true;
            Monitor.PulseAll(this.queueLock);
        }

        this.shutdownEvent.WaitOne();
    }

    /// <summary>
    /// Disposes the thread pool, ensuring a proper shutdown.
    /// </summary>
    public void Dispose()
    {
        if (!this.isShutdown)
        {
            this.Shutdown();
        }

        this.shutdownEvent.Dispose();
    }

    /// <summary>
    /// Queues a task for execution by the thread pool.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <exception cref="InvalidOperationException">Thrown when the thread pool is shutting down.</exception>
    internal void QueueTask(Action action)
    {
        lock (this.queueLock)
        {
            if (this.isShutdown)
            {
                throw new InvalidOperationException("Cannot queue tasks to a shutdown thread pool.");
            }

            this.taskQueue.Enqueue(action);

            Monitor.PulseAll(this.queueLock);
        }
    }

    /// <summary>
    /// Worker thread procedure that processes tasks from the queue.
    /// </summary>
    private void ThreadProc()
    {
        while (true)
        {
            Action task = null!;
            lock (this.queueLock)
            {
                while (this.taskQueue.Count == 0 && !this.isShutdown)
                {
                    Monitor.Wait(this.queueLock);
                }

                if (this.isShutdown && this.taskQueue.Count == 0)
                {
                    break;
                }

                if (this.taskQueue.Count > 0)
                {
                    task = this.taskQueue.Dequeue();
                }
            }

            try
            {
                task();
            }
            catch
            {
                // ignored
            }
        }

        if (Interlocked.Decrement(ref this.threadCount) == 0)
        {
            this.shutdownEvent.Set();
        }
    }
}