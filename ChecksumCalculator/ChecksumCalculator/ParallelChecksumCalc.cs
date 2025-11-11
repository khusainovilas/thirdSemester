// <copyright file="ParallelChecksumCalc.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace ChecksumCalculator;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Calculates a deterministic MD5 checksum of a directory in multithreaded mode.
/// </summary>
public class ParallelChecksumCalc
{
    private static readonly int MaxDegreeOfParallelism = Environment.ProcessorCount;
    private static readonly SemaphoreSlim FileReadSemaphore = new(MaxDegreeOfParallelism, MaxDegreeOfParallelism);

    /// <summary>
    /// Asynchronously calculates a directory checksum using parallelism.
    /// </summary>
    /// <param name="directoryPath">Directory path.</param>
    /// <param name="cancellationToken">Operation cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<byte[]> ComputeChecksumAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        var fullPath = Path.GetFullPath(directoryPath);

        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {fullPath}");
        }

        return await this.ComputeDirectoryHashAsync(fullPath, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously calculates the checksum and returns it as a lowercase string.
    /// </summary>
    /// <param name="directoryPath">Directory path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<string> ComputeChecksumBase64Async(string directoryPath, CancellationToken cancellationToken = default)
    {
        var hash = await this.ComputeChecksumAsync(directoryPath, cancellationToken).ConfigureAwait(false);

        return Convert.ToBase64String(hash);
    }

    private static async ValueTask SemaphoreSlimWaitAsync(CancellationToken cancellationToken)
    {
        await FileReadSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<byte[]> ComputeDirectoryHashAsync(
        string directoryPath,
        CancellationToken cancellationToken)
    {
        var directoryName = Path.GetFileName(directoryPath);
        if (string.IsNullOrEmpty(directoryName))
        {
            directoryName = directoryPath;
        }

        var nameBytes = Encoding.UTF8.GetBytes(directoryName);

        var entries = Directory.GetFileSystemEntries(directoryPath)
            .OrderBy(Path.GetFileName, StringComparer.Ordinal)
            .ToArray();

        var childHashTasks = new List<Task<byte[]>>(entries.Length);

        foreach (var entryPath in entries)
        {
            childHashTasks.Add(
                Directory.Exists(entryPath)
                    ? this.ComputeDirectoryHashAsync(entryPath, cancellationToken)
                    : this.ComputeFileHashAsync(entryPath, cancellationToken));
        }

        var childHashes = await Task.WhenAll(childHashTasks).ConfigureAwait(false);

        var totalLength = nameBytes.Length + childHashes.Sum(hash => hash.Length);
        var combinedBuffer = new byte[totalLength];

        Buffer.BlockCopy(nameBytes, 0, combinedBuffer, 0, nameBytes.Length);

        var currentOffset = nameBytes.Length;
        foreach (var childHash in childHashes)
        {
            Buffer.BlockCopy(childHash, 0, combinedBuffer, currentOffset, childHash.Length);
            currentOffset += childHash.Length;
        }
    
        using var md5 = MD5.Create();
        return md5.ComputeHash(combinedBuffer);
    }

    private async Task<byte[]> ComputeFileHashAsync(string filePath, CancellationToken cancellationToken)
    {
        var fileName = Path.GetFileName(filePath);
        var nameBytes = Encoding.UTF8.GetBytes(fileName);

        await SemaphoreSlimWaitAsync(cancellationToken);

        try
        {
            await using var fileStream = File.OpenRead(filePath);
            using var memoryStream = new MemoryStream(nameBytes.Length + (int)fileStream.Length);

            memoryStream.Write(nameBytes);
            await fileStream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
            memoryStream.Position = 0;
            using var md5 = MD5.Create();
            return await md5.ComputeHashAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            FileReadSemaphore.Release();
        }
    }
}