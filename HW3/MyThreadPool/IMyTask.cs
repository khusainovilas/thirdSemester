// <copyright file="IMyTask.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Defines a contract for a task in the custom thread pool.
/// </summary>
/// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
public interface IMyTask<out TResult>
{
    /// <summary>
    /// Gets a value indicating whether returns true if the task is completed (successfully or with an error).
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// Gets the result of the task.
    /// If the task is not completed, it blocks the calling thread until it is completed.
    /// If the task completes with an exception, it throws an AggregateException with an internal exception.
    /// </summary>
    TResult Result { get; }

    /// <summary>
    /// Creates a new task that runs after the current one is completed,
    /// using its result as an argument for continuation.
    /// A new task is placed in the pool and does not block the calling thread.
    /// </summary>\
    /// <typeparam name="TNewResult">The type of the result produced by the continuation task.</typeparam>
    /// <param name="continuation">A function that takes the result of the current task and produces a new result.</param>
    /// <returns>A new task representing the continuation.</returns>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
}