// <copyright file="MatrixFormatException.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MatrixMultiplication;

/// <summary>
/// The exception that is thrown when a matrix file has an invalid format.
/// </summary>
public class MatrixFormatException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixFormatException"/> class.
    /// </summary>
    public MatrixFormatException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixFormatException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public MatrixFormatException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixFormatException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public MatrixFormatException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}