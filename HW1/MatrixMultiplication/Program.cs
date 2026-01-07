// <copyright file="Program.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

using MatrixMultiplication;

// 1. To multiply matrices from files:
// Write dotnet run -- <matrixFile1> <matrixFile2> <resultFile>
// Input files must contain matrices as strings of numbers separated by spaces.
// Each line of the file is a row of the matrix.
// The program checks the correctness of the matrix sizes and creates a new file with the product.
// Example:
// dotnet run -- "TestMatrix/TestFile1.txt" "TestMatrix/TestFile2.txt" "TestMatrix/result.txt"
//
// 2. To run the statistics benchmark:
//    dotnet run -- benchmark
//
if (args.Length == 1 && args[0].Equals("benchmark", StringComparison.CurrentCultureIgnoreCase))
{
    MatrixBench.RunBenchmark();
}
else if (args.Length != 3 || string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]))
{
    Console.WriteLine("Invalid arguments");
    Console.WriteLine(
        "Usage:\n" +
        "1) Matrix multiplication:\n" +
        "   dotnet run -- <matrixFile1> <matrixFile2> <resultFile>\n" +
        "2) Run benchmark:\n" +
        "   dotnet run -- benchmark");
}
else
{
    var matrixPath1 = args[0];
    var matrixPath2 = args[1];
    var resultFile = args[2];

    try
    {
        var matrix1 = MatrixUtils.ReadMatrixFromFile(matrixPath1);
        var matrix2 = MatrixUtils.ReadMatrixFromFile(matrixPath2);
        var matrixResult = MatrixUtils.MultiplyMatrixParallel(matrix1, matrix2);

        MatrixUtils.WriteMatrixToFile(resultFile, matrixResult);

        Console.WriteLine($"Matrix multiplication completed successfully. Result saved in '{resultFile}'.");
    }
    catch (MatrixFormatException ex)
    {
        Console.WriteLine($"Matrix error: {ex.Message}");
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine($"File error: {ex.Message}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Multiplication error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
    }
}
