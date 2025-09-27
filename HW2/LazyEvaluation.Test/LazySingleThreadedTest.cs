// <copyright file="LazySingleThreadedTest.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace LazyEvaluation.Test;

/// <summary>
/// Unit tests for LazySingleThreaded implementation.
/// </summary>
public class LazySingleThreadedTest : LazyEvaluationTest
{
    /// <summary>
    /// Test for correct work Get() with double.
    /// </summary>
    [Test]
    public void LazySingleThreaded_Get_Double_ReturnsCorrectValue()
    {
        var lazy = this.CreateLazy((Func<double>)Supplier);

        var result = lazy.Get();

        Assert.That(result, Is.EqualTo(42.5));
        return;
        double Supplier() => 42.5;
    }

    /// <summary>
    /// Test for handling exception from supplier in Get().
    /// </summary>
    [Test]
    public void LazySingleThreaded_Get_Double_ThrowsException()
    {
        var lazy = this.CreateLazy((Func<double>)Supplier);

        Assert.Throws<InvalidOperationException>(() => lazy.Get());
        return;
        double Supplier() => throw new InvalidOperationException("Supplier failed");
    }

    /// <inheritdoc/>
    protected override ILazy<T> CreateLazy<T>(Func<T?> supplier)
        where T : default
    {
        return new LazySingleThreaded<T>(supplier!);
    }
}