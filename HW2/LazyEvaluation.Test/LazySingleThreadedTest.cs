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
        var callCount = 0;

        var lazy = this.CreateLazy((Func<double>)Supplier);
        var result1 = lazy.Get();
        var result2 = lazy.Get(); // Второй вызов для проверки кэширования

        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo(1.5));
            Assert.That(result2, Is.EqualTo(1.5));
            Assert.That(callCount, Is.EqualTo(1), "Supplier should be called exactly once.");
        });
        return;

        double Supplier()
        {
            callCount++;
            return 1.5;
        }
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
    protected override ILazy<T?> CreateLazy<T>(Func<T?>? supplier)
        where T : default
    {
        return new LazySingleThreaded<T?>(supplier!);
    }
}