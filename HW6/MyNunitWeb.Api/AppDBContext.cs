// <copyright file="AppDbContext.cs" company="MyNunitWeb">
// Copyright (c) MyNunitWeb. All rights reserved.
// </copyright>

namespace MyNunitWeb.Api;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Entity Framework database context for MyNUnit test runs.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">Database context options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the collection of test runs.
    /// </summary>
    public DbSet<TestRun> TestRuns => this.Set<TestRun>();

    /// <summary>
    /// Gets the collection of test results.
    /// </summary>
    public DbSet<TestResult> TestResults => this.Set<TestResult>();
}