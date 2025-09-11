// <copyright file="matrixmultiplication.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MatrixMultiplication
{
    /// <summary>
    /// Realization of matrix multiplication of parallel processing.
    /// </summary>
    public static class Matrix
    {
        public static void RunExample()
        {
            var matrix1 = new int[,]
            {
                { 1, 2 },
                { 3, 4 },
            };
            var matrix2 = new int[,]
            {
                { 2, 0 },
                { 1, 2 },
            };

            Console.WriteLine("Матрица 1:");
            MatrixUtils.MatrixOutput(matrix1);

            Console.WriteLine("\nМатрица 2:");
            MatrixUtils.MatrixOutput(matrix2);

            var result = MatrixUtils.MultiplyMatrixParallel(matrix1, matrix2);

            Console.WriteLine("\nРезультат умножения:");
            MatrixUtils.MatrixOutput(result);
        }
    }
}