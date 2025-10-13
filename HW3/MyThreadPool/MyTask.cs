// <copyright file="MyTask.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyThreadPool;

internal class MyTask<TResult> : IMyTask<TResult>
{
    private readonly object @lock = new();
    private readonly ManualResetEvent waitHandle = new(false);
    private readonly MyThreadPool pool;
    private readonly Func<TResult> func;
    private TResult result = default!;
    private Exception exception = null!;
    private volatile bool isCompleted;
    private List<Action>? continuations;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
    /// </summary>
    /// <param name="pool">The thread pool to queue continuations.</param>
    /// <param name="func">The function to execute to produce the result.</param>
    public MyTask(MyThreadPool pool, Func<TResult> func)
    {
        this.pool = pool ?? throw new ArgumentNullException(nameof(pool));
        this.func = func ?? throw new ArgumentNullException(nameof(func));
        this.continuations = [];
    }

    /// <inheritdoc/>
    public bool IsCompleted => this.isCompleted;

    /// <inheritdoc/>
    public TResult Result
    {
        get
        {
            if (!this.isCompleted)
            {
                this.waitHandle.WaitOne();
            }

            lock (this.@lock)
            {
                if (this.exception != null)
                {
                    throw new AggregateException("Task failed with an exception.", this.exception);
                }

                return this.result;
            }
        }
    }

    /// <inheritdoc/>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
    {
        ArgumentNullException.ThrowIfNull(continuation);

        var newTask = new MyTask<TNewResult>(this.pool, () => continuation(this.Result));
        lock (this.@lock)
        {
            if (this.isCompleted)
            {
                this.pool.QueueTask(newTask.GetExecuteAction());
            }
            else
            {
                if (this.continuations == null)
                {
                    throw new InvalidOperationException("Cannot add continuation to a completed task with cleared continuations.");
                }

                this.continuations.Add(newTask.GetExecuteAction());
            }
        }

        return newTask;
    }

    /// <summary>
    /// Returns an Action that executes the task, stores the result or exception, and queues continuations.
    /// </summary>
    /// <returns>An Action to be executed by the thread pool.</returns>
    private Action GetExecuteAction()
    {
        return () =>
        {
            try
            {
                this.result = this.func();
            }
            catch (Exception ex)
            {
                lock (this.@lock)
                {
                    this.exception = ex;
                }
            }
            finally
            {
                lock (this.@lock)
                {
                    this.isCompleted = true;
                    this.waitHandle.Set();
                    if (this.continuations != null)
                    {
                        foreach (var continuation in this.continuations)
                        {
                            this.pool.QueueTask(continuation);
                        }
                    }

                    this.continuations = null;
                }
            }
        };
    }
}