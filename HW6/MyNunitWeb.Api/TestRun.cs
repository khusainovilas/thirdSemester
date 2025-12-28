// <copyright file="TestRun.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNunitWeb.Api;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a single execution of a test assembly.
/// </summary>
public class TestRun
{
    /// <summary>
    /// Gets or sets the identifier of the test run.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the start time of the test run.
    /// </summary>
    [Required]
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the number of passed tests.
    /// </summary>
    [Required]
    public int Passed { get; set; }

    /// <summary>
    /// Gets or sets the number of failed tests.
    /// </summary>
    [Required]
    public int Failed { get; set; }

    /// <summary>
    /// Gets or sets the number of ignored tests.
    /// </summary>
    [Required]
    public int Ignored { get; set; }

    /// <summary>
    /// Gets or sets the test results of this run.
    /// </summary>
    public List<TestResult> Results { get; set; } = [];
}