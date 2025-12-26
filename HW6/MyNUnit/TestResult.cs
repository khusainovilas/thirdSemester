// <copyright file="TestResult.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNUnit;

using System;

/// <summary>
/// Represents the result of a single test execution.
/// </summary>
public class TestResult
{
    /// <summary>
    /// Gets test full name.
    /// </summary>
    public string TestName { get; init; } = string.Empty;

    /// <summary>
    /// Gets test execution status.
    /// </summary>
    public TestStatus Status { get; init; }

    /// <summary>
    /// Gets test execution duration.
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Gets additional message (failure reason or ignore reason).
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets exception thrown by test.
    /// </summary>
    public Exception? Exception { get; init; }
}

/// <summary>
/// Test execution status.
/// </summary>
public enum TestStatus
{
    /// <summary>
    /// Test passed successfully.
    /// </summary>
    Passed,

    /// <summary>
    /// Test failed.
    /// </summary>
    Failed,

    /// <summary>
    /// Test was ignored.
    /// </summary>
    Ignored,
}