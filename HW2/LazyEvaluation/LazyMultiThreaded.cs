// <copyright file="LazyMultiThreaded.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace LazyEvaluation;

/// <summary>
/// A multi-threaded implementation of the ILazy interface for lazy evaluation.
/// </summary>
/// <typeparam name="T">The type of the value produced by the lazy computation.</typeparam>
public class LazyMultiThreaded<T> : ILazy<T>
{
    private Func<T>? supplier;
    private T value;
    private volatile bool isComputed;
    private readonly object lockObject = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyMultiThreaded{T}"/> class.
    /// </summary>
    /// <param name="supplier">The delegate that produces the value when needed.</param>
    public LazyMultiThreaded(Func<T>? supplier)
    {
        this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
        this.isComputed = false;
    }

    /// <inheritdoc/>
    public T Get()
    {
        if (this.isComputed)
        {
            return this.value;
        }

        lock (this.lockObject)
        {
            if (this.isComputed)
            {
                return this.value;
            }

            var func = this.supplier;
            if (func != null)
            {
                this.value = func();
                this.isComputed = true;
            }

            this.supplier = null;
        }

        return this.value;
    }
}