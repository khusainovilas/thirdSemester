// <copyright file="AppDBContext.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
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

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestRun>()
            .HasMany(r => r.Results)
            .WithOne()
            .HasForeignKey(r => r.TestRunId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestRun>()
            .HasIndex(r => r.StartedAt);

        modelBuilder.Entity<TestResult>()
            .HasIndex(r => r.TestRunId);

        modelBuilder.Entity<TestResult>()
            .Property(r => r.TestName)
            .HasMaxLength(128);

        modelBuilder.Entity<TestResult>()
            .Property(r => r.Message)
            .HasMaxLength(256);
    }
}