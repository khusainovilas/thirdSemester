// <copyright file="MyNUnitDbContext.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNunitWeb.Api;

using Microsoft.EntityFrameworkCore;
using MyNunitWeb.Api.Models;

/// <summary>
/// Represents the Entity Framework Core database context for MyNUnit.
/// </summary>
public class MyNUnitDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyNUnitDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public MyNUnitDbContext(DbContextOptions<MyNUnitDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the DbSet of test runs.
    /// </summary>
    public DbSet<Run> Runs { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet of individual test results.
    /// </summary>
    public DbSet<TestRunResult> TestResults { get; set; } = null!;

    /// <summary>
    /// Configures the EF Core model, including relationships.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestRunResult>()
            .HasOne(tr => tr.Run)
            .WithMany(r => r.Results)
            .HasForeignKey(tr => tr.RunId);
    }
}
