// <copyright file="LazyEvaluationTest.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace LazyEvaluation.Test;

/// <summary>
/// Unit tests for lazy evaluation.
/// </summary>
public abstract class LazyEvaluationTest
{
    /// <summary>
    /// Verifies that the supplier delegate is called exactly one time.
    /// </summary>
    [Test]
    public void Lazy_Get_NonNullSupplier_SupplierCalledOnce()
    {
        var callCount = 0;

        var supplier = new Func<string>(Supplier);
        var lazy = this.CreateLazy(supplier);

        lazy.Get();
        lazy.Get();

        Assert.That(callCount, Is.EqualTo(1));
        return;

        string Supplier()
        {
            callCount++;
            return "test";
        }
    }

    /// <summary>
    /// Verifies that multiple calls to Get() return the same result.
    /// </summary>
    [Test]
    public void Lazy_Get_SimpleString_NonNullSupplier_SameResult()
    {
        var supplier = new Func<string>(Supplier);
        var lazy = this.CreateLazy(supplier);

        var result1 = lazy.Get();
        var result2 = lazy.Get();

        Assert.That(result2, Is.EqualTo(result1));
        return;

        string Supplier()
        {
            return "test";
        }
    }

    /// <summary>
    /// Verifies that constructing with a null supplier throws ArgumentNullException.
    /// </summary>
    [Test]
    public void Lazy_Constructor_NullSupplier_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => this.CreateLazy<string>(null));
    }

    /// <summary>
    /// Verifies that Get() correctly handles a null result from the supplier.
    /// </summary>
    [Test]
    public void Lazy_Get_NullResultSupplier_ReturnsNull()
    {
        var supplier = new Func<string>(Supplier);
        var lazy = this.CreateLazy(supplier);

        var result = lazy.Get();

        Assert.That(result, Is.Null);
        return;

        string Supplier()
        {
            return null;
        }
    }

    /// <summary>
    /// abstract method for creating an ILazy instance.
    /// </summary>
    /// <param name="supplier">function responsible for computing the value.</param>
    /// <typeparam name="T">the type of the computed value.</typeparam>
    /// <returns>a new ILazy instance.</returns>
    protected abstract ILazy<T> CreateLazy<T>(Func<T?> supplier);
}