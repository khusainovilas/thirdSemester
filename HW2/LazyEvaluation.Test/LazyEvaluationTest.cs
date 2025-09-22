// <copyright file="LazyEvaluationTest.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace LazyEvaluation.Test;

using LazyEvaluation;

/// <summary>
/// Unit tests for lazy evaluation.
/// </summary>
public abstract class LazyEvaluationTest<T>
{
    /// <summary>
    /// Verifies that the supplier is called exactly once.
    /// </summary>
    [Test]
    public void Lazy_Get_SupplierCalledOnce()
    {
        var callCount = 0;

        var lazy = this.CreateLazy(Supplier);

        lazy.Get();
        lazy.Get();

        Assert.That(callCount, Is.EqualTo(1));
        return;

        T Supplier()
        {
            callCount++;
            return default!;
        }
    }

    /// <summary>
    /// Verifies that multiple calls to Get() return the same result.
    /// </summary>
    [Test]
    public void Lazy_Get_SameResult()
    {
        var lazy = this.CreateLazy(Supplier);

        var result1 = lazy.Get();
        var result2 = lazy.Get();

        Assert.That(result2, Is.EqualTo(result1));
        return;
        T? Supplier() => default;
    }

    /// <summary>
    /// Verifies that constructing with a null supplier throws ArgumentNullException.
    /// </summary>
    [Test]
    public void Lazy_Constructor_NullSupplier_NonNullSupplier_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => this.CreateLazy(null));
    }

    /// <summary>
    /// Verifies that Get() correctly handles a null result from the supplier.
    /// </summary>
    [Test]
    public void Lazy_Get_String_NullSupplier_ReturnsNull()
    {
        var lazy = this.CreateLazy(Supplier);

        var result = lazy.Get();

        Assert.That(result, Is.EqualTo(default(T)));
        return;
        T? Supplier() => default;
    }

    protected abstract ILazy<T?> CreateLazy(Func<T?>? supplier);
}