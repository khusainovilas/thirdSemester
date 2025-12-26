// <copyright file="TestRun.cs" company="MyNunitWeb">
// Copyright (c) MyNunitWeb. All rights reserved.
// </copyright>

namespace MyNunitWeb.Api;

/// <summary>
/// Represents a single execution of a test assembly.
/// </summary>
public class TestRun
{
    /// <summary>
    /// Gets or sets the identifier of the test run.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the start time of the test run.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the number of passed tests.
    /// </summary>
    public int Passed { get; set; }

    /// <summary>
    /// Gets or sets the number of failed tests.
    /// </summary>
    public int Failed { get; set; }

    /// <summary>
    /// Gets or sets the number of ignored tests.
    /// </summary>
    public int Ignored { get; set; }

    /// <summary>
    /// Gets or sets the test results of this run.
    /// </summary>
    public List<TestResult> Results { get; set; } = [];
}