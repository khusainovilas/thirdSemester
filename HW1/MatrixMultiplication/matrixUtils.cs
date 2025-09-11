namespace MatrixMultiplication;

/// <summary>
/// Helper static class for working with matrices.
/// Contains functions for generating, checking, and other matrix operations.
/// </summary>
public static class MatrixUtils
{
    
    /// <summary>
    /// Printing the matrix to the console.
    /// </summary>
    public static void MatrixOutput(int[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var columns = matrix.GetLength(1);

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                Console.Write(matrix[i, j].ToString().PadLeft(5));
            }
            Console.WriteLine();
        }
    }
    
    /// <summary>
    /// Generates a random integer matrix of the specified size.
    /// </summary>
    public static int[,] GeneratorRandomMatrix(int rows, int columns, int minValue = -10, int maxValue = 10)
    {
        var matrix = new int[rows, columns];
        var randomNum = new Random();

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                matrix[i, j] = randomNum.Next(minValue, maxValue);
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
    public static int[,] MatrixMultiply(int[,] matrix1, int[,] matrix2)
    {
        var lengthRowMatrix1 = matrix1.GetLength(0);
        var lengthColumnMatrix1 = matrix1.GetLength(1);
        var lengthRowMatrix2 = matrix2.GetLength(0);
        var lengthColumnMatrix2 = matrix2.GetLength(1);

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

        if (lengthColumnMatrix1 != lengthRowMatrix2)
        {
            throw new ArgumentException("The number of columns of the first matrix is not equal to the number of rows of the second one!");
        }
        
        var result = new int[lengthRowMatrix1, lengthColumnMatrix2];
        var threads = new Thread[lengthRowMatrix1];
        
        for (var i = 0; i < lengthRowMatrix1; i++)
        {
            var row = i;
            threads[row] = new Thread(() =>
            {
                for (var j = 0; j < lengthColumnMatrix2; j++)
                {
                    var sum = 0;
                    for (var k = 0; k < lengthColumnMatrix1; k++)
                    {
                        sum += matrix1[row, k] * matrix2[k, j];
                    }
                    result[row, j] = sum;
                }
            });
            threads[row].Start();
        }
        
        for (var i = 0; i < lengthRowMatrix1; i++)
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
    public static bool MatrixEquals(int[,] matrix1, int[,] matrix2)
    {
        if (matrix1.GetLength(0) != matrix2.GetLength(0) ||
            matrix1.GetLength(1) != matrix2.GetLength(1))
        {
            return false;
        }

        int rows = matrix1.GetLength(0);
        int cols = matrix1.GetLength(1);
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (matrix1[i, j] != matrix2[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }
}