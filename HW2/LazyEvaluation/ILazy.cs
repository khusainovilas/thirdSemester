// <copyright file="ILazy.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace LazyEvaluation;

/// <summary>
/// Defines a contract for lazy evaluation, where a value of type T is computed only on the first call and cached for subsequent access.
/// </summary>
/// <typeparam name="T">The type of the value to be lazily computed.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Gets the lazily computed value. The value is computed on the first call and cached for subsequent calls.
    /// </summary>
    /// <returns>The computed or cached value.</returns>
    T Get();
}