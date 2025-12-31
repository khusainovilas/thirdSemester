// <copyright file="Run.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNunitWeb.Api.Models;

/// <summary>
/// Represents a test run containing metadata and the results of executed tests.
/// </summary>
public class Run
{
    /// <summary>
    /// Gets or sets the unique identifier of the run.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the run was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the names of the assemblies.
    /// </summary>
    public string AssemblyNames { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of test results for this run.
    /// </summary>
    public List<TestRunResult> Results { get; set; } = new ();
}