// <copyright file="TestController.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNunitWeb.Api;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNUnit;

/// <summary>
/// Controller for uploading assemblies and running tests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly AppDbContext dbContext;
    private readonly IWebHostEnvironment env;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestController"/> class.
    /// </summary>
    /// <param name="dbContext">The context of the database.</param>
    /// <param name="env">The execution environment of the web app.</param>
    public TestController(AppDbContext dbContext, IWebHostEnvironment env)
    {
        this.dbContext = dbContext;
        this.env = env;
    }

    /// <summary>
    /// Uploads DLL files, runs MyNUnit tests, and saves the results in the database.
    /// </summary>
    /// <param name="files">The uploaded DLL files.</param>
    /// <returns>Information about the test run.</returns>
    [HttpPost("run")]
    public async Task<IActionResult> RunTests([FromForm] IFormFileCollection files)
    {
        if (!files.Any())
            return BadRequest("No files uploaded.");

        var tempFolder = Path.Combine(env.ContentRootPath, "TempUploads");
        Directory.CreateDirectory(tempFolder);

        var testRuns = new List<TestRun>();

        foreach (var file in files)
        {
            var filePath = Path.Combine(tempFolder, file.FileName);
            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            var testClasses = TestDiscovery.DiscoverFromDll(filePath);

            if (!testClasses.Any())
                continue;

            var results = TestRunner.Run(testClasses);

            var testRun = new TestRun
            {
                StartedAt = DateTime.UtcNow,
                Passed = results.Count(r => r.Status == TestStatus.Passed),
                Failed = results.Count(r => r.Status == TestStatus.Failed),
                Ignored = results.Count(r => r.Status == TestStatus.Ignored),
                Results = results.Select(r => new TestResult
                {
                    TestName = r.TestName,
                    Status = r.Status,
                    DurationMs = r.Duration.TotalMilliseconds,
                    Message = r.Message,
                }).ToList(),
            };

            dbContext.TestRuns.Add(testRun);
            testRuns.Add(testRun);
        }

        await dbContext.SaveChangesAsync();

        if (!testRuns.Any())
            return BadRequest("No tests found in uploaded assemblies.");

        return Ok(testRuns);
    }

    /// <summary>
    /// Returns all test runs with their results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var runs = await dbContext.TestRuns
            .Include(r => r.Results)
            .OrderByDescending(r => r.StartedAt)
            .ToListAsync();

        return Ok(runs);
    }
}
