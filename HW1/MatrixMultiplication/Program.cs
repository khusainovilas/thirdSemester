// <copyright file="Program.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

using MatrixMultiplication;

// Write dotnet run -- <matrixFile1> <matrixFile2> <resultFile>
// Input files must contain matrices as strings of numbers separated by spaces.
// Each line of the file is a row of the matrix.
// The program checks the correctness of the matrix sizes and creates a new file with the product.
// Example:
// dotnet run -- "TestMatrix/TestFile1.txt" "TestMatrix/TestFile2.txt" "TestMatrix/result.txt"
//
if (args.Length != 3 || string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]))
{
    Console.WriteLine("Error when passing arguments");
    return;
}

var matrixPath1 = args[0];
var matrixPath2 = args[1];
var resultFile = args[2];

var matrix1 = MatrixUtils.ReadMatrixFromFile(matrixPath1);
var matrix2 = MatrixUtils.ReadMatrixFromFile(matrixPath2);
var matrixResult = MatrixUtils.MultiplyMatrixParallel(matrix1, matrix2);

MatrixUtils.WriteMatrixToFile(resultFile, matrixResult);