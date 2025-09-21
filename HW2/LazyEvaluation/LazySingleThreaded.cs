// <copyright file="LazySingleThreaded.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace LazyEvaluation;

/// <summary>
/// A single-threaded implementation of the ILazy interface for lazy evaluation.
/// </summary>
/// <typeparam name="T">The type of the value produced by the lazy computation.</typeparam>
public class LazySingleThreaded<T> : ILazy<T>
{
    private Func<T>? supplier;
    private T value;
    private bool isComputed;

    /// <summary>
    /// Initializes a new instance of the <see cref="LazySingleThreaded{T}"/> class.
    /// </summary>
    /// <param name="supplier">The delegate that produces the value when needed.</param>
    public LazySingleThreaded(Func<T> supplier)
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

        if (this.supplier != null)
        {
            this.value = this.supplier();
            this.isComputed = true;
        }

        this.supplier = null;
        return this.value;
    }
}