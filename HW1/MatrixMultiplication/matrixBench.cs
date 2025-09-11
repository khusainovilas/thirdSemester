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
public class MatrixBench
{
    /// <summary>
    /// Runs the benchmark for multiple matrix sizes and saves the results to a file.
    /// </summary>
    public static void RunBenchmark()
    {
        int[] sized = [100, 200, 500];
        const string filePath = "resultBenchmark.txt";

        File.WriteAllText(filePath, "Size\tSeq_Mean\tSeq_Sigma\tPar_Mean\tPar_Sigma\n");

        foreach (var size in sized)
        {
            var sequentialResult = BenchmarkMatrixMultiplication(size, false);
            var parallelResult = BenchmarkMatrixMultiplication(size, true);

            // {значение, ширина} — задаёт фиксированную ширину колонки
            var line =
                $"{size,4} {sequentialResult.Mean,10:F4} {sequentialResult.Sigma,10:F4} {parallelResult.Mean,10:F4} {parallelResult.Sigma,10:F4}";

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
    /// Tuple (Mean, Sigma):
    /// Mean — mathematical expectation;
    /// Sigma — standard deviation.</returns>
    private static (double Mean, double Sigma) BenchmarkMatrixMultiplication(int size, bool useParallel, int repeat = 10)
    {
        var elapsedTimes = new double[10];
        for (var i = 0; i < repeat; i++)
        {
            elapsedTimes[i] = PerformSingleRun(size, useParallel);
        }

        var mean = elapsedTimes.Average();
        var sigma = Math.Sqrt(elapsedTimes.Average(t => Math.Pow(t - mean, 2)));

        return (mean, sigma);
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

        _ = useParallel
            ? MatrixUtils.MultiplyMatrixParallel(matrix1, matrix2)
            : MatrixUtils.MatrixMultiply(matrix1, matrix2);

        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }
}