// <copyright file="TestController.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNunitWeb.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNUnit;
using MyNunitWeb.Api.Models;

/// <summary>
/// API controller for running tests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IWebHostEnvironment env;
    private readonly MyNUnitDbContext db;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestController"/> class.
    /// </summary>
    /// <param name="env">The web hosting environment.</param>
    /// <param name="db">The database context for MyNUnit.</param>
    public TestController(IWebHostEnvironment env, MyNUnitDbContext db)
    {
        this.env = env;
        this.db = db;
    }

    /// <summary>
    /// Uploads assemblies and runs tests in them.
    /// </summary>
    /// <param name="files">Array of uploaded DLL files.</param>
    /// <returns>Returns a JSON object containing the run ID, timestamp, assembly names, and results.</returns>
    [HttpPost("run")]
    public async Task<IActionResult> Run([FromForm] IFormFile[]? files)
    {
        if (files == null || files.Length == 0)
        {
            return this.BadRequest("No files");
        }

        var runId = Guid.NewGuid().ToString();
        var uploadDir = Path.Combine(this.env.ContentRootPath, "uploads", runId);
        Directory.CreateDirectory(uploadDir);

        var assemblyPaths = new List<string>();
        var assemblyNames = new List<string>();

        foreach (var file in files)
        {
            if (file.FileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                var path = Path.Combine(uploadDir, file.FileName);
                await using var stream = System.IO.File.Create(path);
                await file.CopyToAsync(stream);
                assemblyPaths.Add(path);
                assemblyNames.Add(file.FileName);
            }
        }

        if (assemblyPaths.Count == 0)
        {
            return this.BadRequest("There is no DLL");
        }

        var testClasses = new List<Type>();
        foreach (var path in assemblyPaths)
        {
            try
            {
                var classes = TestDiscovery.DiscoverFromDll(path);
                testClasses.AddRange(classes);
            }
            catch
            {
            }
        }

        if (testClasses.Count == 0)
        {
            return this.BadRequest("There are no tests");
        }

        var results = TestRunner.Run(testClasses);

        var run = new Run
        {
            AssemblyNames = string.Join(", ", assemblyNames),
            Results = results.Select(r => new TestRunResult
            {
                TestName = r.TestName,
                Status = r.Status,
                Duration = r.Duration,
                Message = r.Message,
            }).ToList(),
        };

        this.db.Runs.Add(run);
        await this.db.SaveChangesAsync();

        return this.Ok(new
        {
            id = run.Id,
            timestamp = run.Timestamp,
            assemblyNames = run.AssemblyNames,
            results = run.Results.Select(r => new
            {
                testName = r.TestName,
                status = r.Status.ToString(),
                duration = r.Duration.TotalMilliseconds,
                message = r.Message ?? string.Empty,
            }),
        });
    }

    /// <summary>
    /// Retrieves the history of all test runs.
    /// </summary>
    /// <returns>Returns a list of test runs including counts of passed, failed, and ignored tests.</returns>
    [HttpGet("history")]
    public IActionResult History()
    {
        var runs = this.db.Runs
            .Include(r => r.Results)
            .OrderByDescending(r => r.Timestamp)
            .Select(r => new
            {
                id = r.Id,
                timestamp = r.Timestamp,
                assemblyNames = r.AssemblyNames,
                passed = r.Results.Count(x => x.Status == TestStatus.Passed),
                failed = r.Results.Count(x => x.Status == TestStatus.Failed),
                ignored = r.Results.Count(x => x.Status == TestStatus.Ignored),
            })
            .ToList();

        return this.Ok(runs);
    }

    /// <summary>
    /// Retrieves details of a specific test run by ID.
    /// </summary>
    /// <param name="id">The ID of the test run.</param>
    /// <returns>Returns a JSON object with run details and results.</returns>
    [HttpGet("run/{id}")]
    public IActionResult GetRun(int id)
    {
        var run = this.db.Runs
            .Include(r => r.Results)
            .FirstOrDefault(r => r.Id == id);

        if (run == null)
        {
            return this.NotFound();
        }

        return this.Ok(new
        {
            id = run.Id,
            timestamp = run.Timestamp,
            assemblyNames = run.AssemblyNames,
            results = run.Results.Select(r => new
            {
                testName = r.TestName,
                status = r.Status.ToString(),
                duration = r.Duration.TotalMilliseconds,
                message = r.Message ?? string.Empty,
            }),
        });
    }
}