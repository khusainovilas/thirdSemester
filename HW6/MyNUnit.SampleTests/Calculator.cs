// <copyright file="Calculator.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNUnit.SampleTests;

/// <summary>
/// Simple calculator class.
/// </summary>
public static class Calculator
{
    /// <summary>
    /// Adds two numbers.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns>The sum of a and b.</returns>
    public static int Add(int a, int b) => a + b;

    /// <summary>
    /// Subtracts second number from first.
    /// </summary>
    /// <param name="a">The number to subtract from.</param>
    /// <param name="b">The number to subtract.</param>
    /// <returns>The result of a minus b.</returns>
    public static int Subtract(int a, int b) => a - b;

    /// <summary>
    /// Multiplies two numbers.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns>The product of a and b.</returns>
    public static int Multiply(int a, int b) => a * b;

    /// <summary>
    /// Divides first number by second.
    /// </summary>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The result of a divided by b.</returns>
    public static int Divide(int a, int b)
    {
        return b == 0 ? throw new DivideByZeroException() : a / b;
    }
}