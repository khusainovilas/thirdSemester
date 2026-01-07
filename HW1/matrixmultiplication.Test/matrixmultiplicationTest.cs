// <copyright file="MatrixmultiplicationTest.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace Matrixmultiplication.Test;

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
    /// </summary>
    [Test]
    public void MatrixUtils_MatrixMultiply_Matrix1_Matrix2()
    {
        var result = MatrixUtils.MultiplyMatrix(this.matrix1, this.matrix2);
        Assert.That(MatrixUtils.AreMatrixEqual(result, this.matrixExpected), Is.True);
    }

    /// <summary>
    /// Tests the parallel matrix multiplication function.
    /// </summary>
    [Test]
    public void MatrixUtils_MultiplyMatrixParallel_Matrix1_Matrix2()
    {
        var result = MatrixUtils.MultiplyMatrixParallel(this.matrix1, this.matrix2);
        Assert.That(MatrixUtils.AreMatrixEqual(result, this.matrixExpected), Is.True);
    }

    /// <summary>
    /// Tests multiplication with 1x1 matrices.
    /// </summary>
    [Test]
    public void MultiplyMatrix_OneByOneMatrices_ReturnsCorrectValue()
    {
        int[,] a = { { 5 } };
        int[,] b = { { 3 } };
        int[,] expected = { { 15 } };

        var resultSeq = MatrixUtils.MultiplyMatrix(a, b);
        var resultPar = MatrixUtils.MultiplyMatrixParallel(a, b);

        Assert.Multiple(() =>
        {
            Assert.That(MatrixUtils.AreMatrixEqual(resultSeq, expected), Is.True);
            Assert.That(MatrixUtils.AreMatrixEqual(resultPar, expected), Is.True);
        });
    }

    /// <summary>
    /// Tests multiplication with empty matrices.
    /// </summary>
    [Test]
    public void MultiplyMatrix_EmptyMatrices_ThrowsArgumentException()
    {
        int[,] empty = new int[0, 0];

        Assert.Throws<ArgumentException>(() => MatrixUtils.MultiplyMatrix(empty, this.matrix2));
        Assert.Throws<ArgumentException>(() => MatrixUtils.MultiplyMatrixParallel(empty, this.matrix2));
    }
}
