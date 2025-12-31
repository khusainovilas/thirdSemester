// <copyright file="CalculatorTests.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNUnit.SampleTests;

using System;

/// <summary>
/// Tests for the <see cref="Calculator"/> class using MyNUnit.
/// </summary>
public class CalculatorTests
{
    /// <summary>
    /// Checks the addition of two numbers.
    /// </summary>
    [Test]
    public void Calculator_Add_ShouldReturnSum()
    {
        var result = Calculator.Add(2, 3);
        if (result != 5)
        {
            throw new InvalidOperationException($"Expected 5, but got {result}");
        }
    }

    /// <summary>
    /// Checks the subtraction of two numbers.
    /// </summary>
    [Test]
    public void Calculator_Subtract_ShouldReturnDifference()
    {
        var result = Calculator.Subtract(10, 4);
        if (result != 6)
        {
            throw new InvalidOperationException($"Expected 6, but got {result}");
        }
    }

    /// <summary>
    /// Checks division by zero (should throw an exception).
    /// </summary>
    [Test(Expected = typeof(DivideByZeroException))]
    public void Calculator_Divide_ByZero_ShouldThrow()
    {
        Calculator.Divide(10, 0);
    }

    /// <summary>
    /// Ignored test.
    /// </summary>
    [Test]
    [Ignore("Example of a ignored test")]
    public void Calculator_Multiply_ShouldBeIgnored()
    {
        var result = Calculator.Multiply(3, 4);
        if (result != 12)
        {
            throw new InvalidOperationException($"Expected 12, but got {result}");
        }
    }

    /// <summary>
    /// A special test that intentionally crashes.
    /// </summary>
    [Test]
    public void Calculator_FailedTest_ShouldFail()
    {
        var result = Calculator.Add(1, 1);
        if (result != 3)
        {
            throw new InvalidOperationException("This test is supposed to fail");
        }
    }
}