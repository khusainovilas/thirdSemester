// <copyright file="matrixmultiplicationTest.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace matrixmultiplication.Test;

using MatrixMultiplication;

/// <summary>
/// NUnit tests for matrix multiplication functions.
/// Tests both sequential and parallel implementations for correctness.
/// </summary>
public class MatrixmultiplicationTest
{
    private int[,] matrix1;
    private int[,] matrix2;
    private int[,] matrixExpected;

    /// <summary>
    /// Initializes matrices before each test.
    /// </summary>
    [SetUp] 
    public void Setup()
    {
        this.matrix1 = new int[,]
        {
            { 1, 2 },
            { 3, 4 },
        };
        this.matrix2 = new int[,]
        {
            { 2, 0 },
            { 1, 2 },
        };
        this.matrixExpected = new int[,]
        {
            { 4, 4 },
            { 10, 8 },
        };
    }

    /// <summary>
    /// Tests the sequential matrix multiplication function.
    /// Checks that the result matches the expected output.
    /// </summary>
    [Test]
    public void MatrixMultiply_ReturnsCorrectResult()
    {
        var result = MatrixUtils.MatrixMultiply(matrix1, matrix2);
        Assert.That(MatrixUtils.MatrixEquals(result, matrixExpected), Is.True);
    }

    /// <summary>
    /// Tests the parallel matrix multiplication function.
    /// Ensures that parallel computation produces the correct result.
    /// </summary>
    [Test]
    public void ParallelMatrixMultiply_ReturnsCorrectResult()
    {
        var result = MatrixUtils.MultiplyMatrixParallel(matrix1, matrix2);
        Assert.That(MatrixUtils.MatrixEquals(result, matrixExpected), Is.True); 
    }
}

