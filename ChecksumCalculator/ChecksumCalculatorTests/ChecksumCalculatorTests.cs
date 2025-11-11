// <copyright file="ChecksumCalculatorTests.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace ChecksumCalculatorTests;

using ChecksumCalculator;

/// <summary>
/// Checksum Calculator implementation tests.
/// </summary>
public class ChecksumCalculatorTests
{
    private SequentialChecksumCalc sequential = null!;
    private ParallelChecksumCalc parallel = null!;

    [SetUp]
    public void Setup()
    {
        this.sequential = new SequentialChecksumCalc();
        this.parallel = new ParallelChecksumCalc();
    }

    /// <summary>
    /// Checks that the serial and parallel calculators return the same hash
    /// for a complex directory structure with files, subfolders, and different names.
    /// </summary>
    [Test]
    public async Task SequentialChecksumCalc_And_ParallelChecksumCalc_WithComplexDirectoryStructure_ReturnSameHash()
    {
        var testDirectory = CreateComplexTestDirectory();

        try
        {
            var sequentialHash = await this.sequential.ComputeChecksumBase64Async(testDirectory);
            var parallelHash = await this.parallel.ComputeChecksumBase64Async(testDirectory);

            Assert.That(parallelHash, Is.EqualTo(sequentialHash));
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    /// <summary>
    /// Checks that files with the same content but different names produce different hashes.
    /// </summary>
    [Test]
    public async Task SequentialChecksumCalc_WithSameFileContentButDifferentNames_ReturnsDifferentHashes()
    {
        // Arrange
        var dir1 = Path.Combine(Path.GetTempPath(), "same_content_diff_name_1_" + Guid.NewGuid().ToString("N"));
        var dir2 = Path.Combine(Path.GetTempPath(), "same_content_diff_name_2_" + Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(dir1);
        Directory.CreateDirectory(dir2);

        const string content = "identical content";
        await File.WriteAllTextAsync(Path.Combine(dir1, "first.txt"), content);
        await File.WriteAllTextAsync(Path.Combine(dir2, "second.txt"), content);

        try
        {
            var hash1 = await this.sequential.ComputeChecksumBase64Async(dir1);
            var hash2 = await this.sequential.ComputeChecksumBase64Async(dir2);

            Assert.That(hash2, Is.Not.EqualTo(hash1));
        }
        finally
        {
            Directory.Delete(dir1, recursive: true);
            Directory.Delete(dir2, recursive: true);
        }
    }

    /// <summary>
    /// Checks that the order in which files are created in the file system does not affect the resulting hash.
    /// </summary>
    [Test]
    public async Task SequentialChecksumCalc_FileOrderDoesNotMatter_ReturnsSameHash()
    {
        var dirPath = Path.Combine(Path.GetTempPath(), "file_order_test_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dirPath);

        await File.WriteAllTextAsync(Path.Combine(dirPath, "c.txt"), "third");
        await File.WriteAllTextAsync(Path.Combine(dirPath, "a.txt"), "first");
        await File.WriteAllTextAsync(Path.Combine(dirPath, "b.txt"), "second");

        var hash1 = await this.sequential.ComputeChecksumBase64Async(dirPath);

        Directory.Delete(dirPath, recursive: true);
        Directory.CreateDirectory(dirPath);

        await File.WriteAllTextAsync(Path.Combine(dirPath, "a.txt"), "first");
        await File.WriteAllTextAsync(Path.Combine(dirPath, "b.txt"), "second");
        await File.WriteAllTextAsync(Path.Combine(dirPath, "c.txt"), "third");

        var hash2 = await this.sequential.ComputeChecksumBase64Async(dirPath);

        Assert.That(hash2, Is.EqualTo(hash1));
        Directory.Delete(dirPath, recursive: true);
    }

    /// <summary>
    /// Helper method: creates a complex test directory and file structure.
    /// </summary>
    private static string CreateComplexTestDirectory()
    {
        var root = Path.Combine(Path.GetTempPath(), "complex_checksum_test_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        Directory.CreateDirectory(Path.Combine(root, "subfolder"));
        Directory.CreateDirectory(Path.Combine(root, "кириллица"));

        File.WriteAllText(Path.Combine(root, "hello.txt"), "Hello!");
        File.WriteAllBytes(Path.Combine(root, "empty.bin"), Array.Empty<byte>());
        File.WriteAllText(Path.Combine(root, "subfolder", "deep.txt"), "deep content");
        File.WriteAllText(Path.Combine(root, "кириллица", "заказ.txt"), "Молоко");

        return root;
    }
}