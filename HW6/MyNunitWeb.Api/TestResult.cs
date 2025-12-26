// <copyright file="TestResult.cs" company="MyNunitWeb">
// Copyright (c) MyNunitWeb. All rights reserved.
// </copyright>

namespace MyNunitWeb.Api;

using MyNUnit;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a single test execution result.
/// </summary>
public class TestResult
{
    /// <summary>
    /// Gets or sets the identifier of the test result.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the test name.
    /// </summary>
    [MaxLength(128)]
    public string TestName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the execution status of the test.
    /// </summary>
    public TestStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the test duration in milliseconds.
    /// </summary>
    public double DurationMs { get; set; }

    /// <summary>
    /// Gets or sets the message associated with the test result.
    /// </summary>
    [MaxLength(256)]
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the related test run identifier.
    /// </summary>
    public int TestRunId { get; set; }
}