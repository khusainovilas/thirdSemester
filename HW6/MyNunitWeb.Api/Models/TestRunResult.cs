// <copyright file="TestRunResult.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNunitWeb.Api.Models;

using MyNUnit;

/// <summary>
/// Represents the result of a single test within a test run.
/// </summary>
public class TestRunResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the test result.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the fully qualified name of the test.
    /// </summary>
    public string TestName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the test (Passed, Failed, Ignored).
    /// </summary>
    public TestStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the duration of the test execution.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets or sets the message associated with the test result.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the foreign key of the related <see cref="Run"/>.
    /// </summary>
    public int RunId { get; set; }

    /// <summary>
    /// Gets or sets the related <see cref="Run"/> object.
    /// </summary>
    public Run? Run { get; set; }
}
