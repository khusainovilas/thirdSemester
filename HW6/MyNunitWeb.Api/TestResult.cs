// <copyright file="TestResult.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNunitWeb.Api;

using MyNUnit;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents a single test execution result.
/// </summary>
public class TestResult
{
    /// <summary>
    /// Gets or sets the identifier of the test result.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the test name.
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string TestName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the execution status of the test.
    /// </summary>
    [Required]
    public TestStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the test duration in milliseconds.
    /// </summary>
    [Required]
    public double DurationMs { get; set; }

    /// <summary>
    /// Gets or sets the message associated with the test result.
    /// </summary>
    [MaxLength(256)]
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the related test run identifier.
    /// </summary>
    [ForeignKey("TestRun")]
    public int TestRunId { get; set; }
    
    /// <summary>
    /// Navigation property to the related test run.
    /// </summary>
    public TestRun? TestRun { get; set; }
}