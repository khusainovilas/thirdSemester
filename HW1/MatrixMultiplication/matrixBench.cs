// <copyright file="matrixBench.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MatrixMultiplication;

using System;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Compare the operating speed with the sequential version depending on the matrix sizes.
/// </summary>
public static class MatrixBench
{
    /// <summary>
    /// Runs the benchmark for multiple matrix sizes and saves the results to a file.
    /// </summary>
    public static void RunBenchmark()
    {
        int[] sizes = [100, 200, 300, 400, 500];
        const string filePath = "resultBenchmark.txt";

        File.WriteAllText(filePath, $"{"Size",4} {"Seq_Exp",10} {"Seq_Dev",10} {"Par_Exp",10} {"Par_Dev",10}\n");

        foreach (var size in sizes)
        {
            var sequentialResult = BenchmarkMatrixMultiplication(size, false);
            var parallelResult = BenchmarkMatrixMultiplication(size, true);

            var line =
                $"{size,4} {sequentialResult.Expectation,10:F4} {sequentialResult.Deviation,10:F4} {parallelResult.Expectation,10:F4} {parallelResult.Deviation,10:F4}";

            File.AppendAllText("resultBenchmark.txt", line + Environment.NewLine);
        }
    }

    /// <summary>
    /// Runs a matrix multiplication benchmark and computes mathematical expectation and standard deviation.
    /// </summary>
    /// <param name="size">The number of rows and columns of the square matrix.</param>
    /// <param name="useParallel">If true, uses parallel matrix multiplication.</param>
    /// <param name="repeat">Number of measurement repetitions for statistics (default 100).</param>
    /// <returns>
    /// Tuple (expectation, deviation):.
    /// - Expectation: mathematical expectation time in milliseconds.
    /// - Deviation: standard deviation time in milliseconds.
    /// </returns>
    private static (double Expectation, double Deviation) BenchmarkMatrixMultiplication(int size, bool useParallel, int repeat = 10)
    {
        var elapsedTimes = new double[repeat];
        for (var i = 0; i < repeat; i++)
        {
            elapsedTimes[i] = PerformSingleRun(size, useParallel);
        }

        var expectation = elapsedTimes.Average();
        var deviation = Math.Sqrt(elapsedTimes.Average(t => Math.Pow(t - expectation, 2)));

        return (expectation, deviation);
    }

    /// <summary>
    /// Performs a single run of matrix multiplication with randomly generated matrix.
    /// </summary>
    /// <param name="size">The number of rows and columns of the square matrix.</param>
    /// <param name="useParallel">If true, uses parallel matrix multiplication.</param>
    /// <returns>Elapsed time in milliseconds for a single multiplication.</returns>
    private static long PerformSingleRun(int size, bool useParallel)
    {
        var matrix1 = MatrixUtils.GeneratorRandomMatrix(size, size);
        var matrix2 = MatrixUtils.GeneratorRandomMatrix(size, size);

        var stopwatch = Stopwatch.StartNew();

        if (useParallel)
        {
            MatrixUtils.MultiplyMatrixParallel(matrix1, matrix2);
        }
        else
        {
            MatrixUtils.MatrixMultiply(matrix1, matrix2);
        }

        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }
}