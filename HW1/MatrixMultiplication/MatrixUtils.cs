// <copyright file="MatrixUtils.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MatrixMultiplication;

/// <summary>
/// Helper static class for working with matrices.
/// Contains functions for generating, checking, and other matrix operations.
/// </summary>
public static class MatrixUtils
{
    /// <summary>
    /// Generates a random integer matrix of the specified size with values in the given range.
    /// </summary>
    /// <param name="rows">Number of rows in the matrix.</param>
    /// <param name="columns">Number of columns in the matrix.</param>
    /// <param name="minValue">Minimum value for matrix elements (inclusive).</param>
    /// <param name="maxValue">Maximum value for matrix elements (exclusive).</param>
    /// <returns>A 2D integer array filled with random values.</returns>
    public static int[,] GenerateRandomMatrix(int rows, int columns, int minValue = -100, int maxValue = 100)
    {
        var matrix = new int[rows, columns];
        var randomNumbers = new Random();

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                matrix[i, j] = randomNumbers.Next(minValue, maxValue);
            }
        }

        return matrix;
    }

    /// <summary>
    /// Multiplies two matrices and returns the result.
    /// </summary>
    /// <param name="matrix1">First matrix.</param>
    /// <param name="matrix2">Second matrix.</param>
    /// <returns>Resulting matrix after multiplication.</returns>
    public static int[,] MultiplyMatrix(int[,] matrix1, int[,] matrix2)
    {
        var lengthRowMatrix1 = matrix1.GetLength(0);
        var lengthColumnMatrix1 = matrix1.GetLength(1);
        var lengthRowMatrix2 = matrix2.GetLength(0);
        var lengthColumnMatrix2 = matrix2.GetLength(1);

        // Checking the compatibility of matrices for multiplication
        if (lengthColumnMatrix1 != lengthRowMatrix2)
        {
            throw new ArgumentException("The number of columns of the first matrix is not equal to the number of rows of the second one!");
        }

        var result = new int[lengthRowMatrix1, lengthColumnMatrix2];

        for (var i = 0; i < lengthRowMatrix1; i++)
        {
            for (var j = 0; j < lengthColumnMatrix2; j++)
            {
                var sum = 0;
                for (var k = 0; k < lengthColumnMatrix1; k++)
                {
                    sum += matrix1[i, k] * matrix2[k, j];
                }

                result[i, j] = sum;
            }
        }

        return result;
    }

    /// <summary>
    /// Multiplies two matrices in parallel.
    /// Each cell of the result matrix is computed in a separate thread.
    /// </summary>
    /// <param name="matrix1">First matrix.</param>
    /// <param name="matrix2">Second matrix.</param>
    /// <returns>Resulting matrix of multiplication.</returns>
    /// <exception cref="ArgumentException">Thrown when dimensions are not compatible for multiplication.</exception>
    public static int[,] MultiplyMatrixParallel(int[,] matrix1, int[,] matrix2)
    {
        var lengthRowMatrix1 = matrix1.GetLength(0);
        var lengthColumnMatrix1 = matrix1.GetLength(1);
        var lengthRowMatrix2 = matrix2.GetLength(0);
        var lengthColumnMatrix2 = matrix2.GetLength(1);

        // Checking the compatibility of matrices for multiplication
        if (lengthColumnMatrix1 != lengthRowMatrix2)
        {
            throw new ArgumentException("The number of columns of the first matrix is not equal to the number of rows of the second one.");
        }

        var result = new int[lengthRowMatrix1, lengthColumnMatrix2];
        var numThreads = Math.Min(Environment.ProcessorCount, lengthRowMatrix1);
        var threads = new Thread[numThreads];

        for (var t = 0; t < numThreads; t++)
        {
            var threadIndex = t;
            threads[t] = new Thread(() =>
            {
                for (var i = threadIndex; i < lengthRowMatrix1; i += numThreads)
                {
                    for (var j = 0; j < lengthColumnMatrix2; j++)
                    {
                        var sum = 0;
                        for (var k = 0; k < lengthColumnMatrix1; k++)
                        {
                            sum += matrix1[i, k] * matrix2[k, j];
                        }

                        result[i, j] = sum;
                    }
                }
            });
            threads[t].Start();
        }

        for (var i = 0; i < numThreads; i++)
        {
            threads[i].Join();
        }

        return result;
    }

    /// <summary>
    /// Compares two matrices for equality.
    /// </summary>
    /// <param name="matrix1">First matrix.</param>
    /// <param name="matrix2">Second matrix.</param>
    /// <returns>
    /// true — if the matrices are the same size and all their elements match.
    /// false — if the dimensions are different or at least one element is different.
    /// </returns>
    public static bool AreMatrixEqual(int[,] matrix1, int[,] matrix2)
    {
        if (matrix1.GetLength(0) != matrix2.GetLength(0) ||
            matrix1.GetLength(1) != matrix2.GetLength(1))
        {
            return false;
        }

        var rows = matrix1.GetLength(0);
        var columns = matrix1.GetLength(1);

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                if (matrix1[i, j] != matrix2[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Reads a matrix of integers from a text file.
    /// </summary>
    /// <param name="path">The path to the file containing the matrix.</param>
    /// <returns>matrix from file.</returns>
    public static int[,] ReadMatrixFromFile(string path)
    {
        // Checking the existence of the file
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found.");
        }

        var lines = File.ReadAllLines(path);

        // Checking that the file is not empty
        if (lines.Length == 0)
        {
            throw new MatrixFormatException("Matrix file is empty.");
        }

        var rows = lines.Length;
        var columns = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        var matrix = new int[rows, columns];

        for (var i = 0; i < rows; i++)
        {
            // Check that the string contains only numbers, spaces, and minus signs.
            if (lines[i].Any(c => !char.IsDigit(c) && c != ' ' && c != '-'))
            {
                throw new MatrixFormatException("Invalid character in matrix file: only digits, spaces and minus signs are allowed.");
            }

            var nums = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            // Checking that all rows and columns in the matrix have the same length
            if (nums.Length != columns)
            {
                throw new MatrixFormatException("Invalid matrix format: the matrix is not complete.");
            }

            for (var j = 0; j < columns; j++)
            {
                matrix[i, j] = nums[j];
            }
        }

        return matrix;
    }

    /// <summary>
    /// Writes a matrix to a text file.
    /// </summary>
    /// <param name="path">The path to the output file.</param>
    /// <param name="matrix">Matrix for writing to file to write.</param>
    public static void WriteMatrixToFile(string path, int[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        using var writer = new StreamWriter(path);
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                writer.Write(matrix[i, j]);
                if (j < columns - 1)
                {
                    writer.Write(" ");
                }
            }

            writer.WriteLine();
        }
    }
}