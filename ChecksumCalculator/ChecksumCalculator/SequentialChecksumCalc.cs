// <copyright file="SequentialChecksumCalc.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace ChecksumCalculator;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Calculates a MD5 checksum of a directory in sequential mode.
/// </summary>
public class SequentialChecksumCalc
{
    private readonly MD5 md5 = MD5.Create();

    /// <summary>
    /// Asynchronously calculates a deterministic checksum of a directory (single-threaded across tasks).
    /// </summary>
    /// /// <param name="directoryPath">
    /// Full or relative path to the directory for which the hash sum should be calculated.
    /// </param>
    /// <param name="cancellationToken">
    /// Operation cancellation token. Allows you to abort a lengthy calculation.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<byte[]> ComputeChecksumAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        var fullPath = Path.GetFullPath(directoryPath);

        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {fullPath}");
        }

        return await this.ComputeDirectoryHashAsync(fullPath, cancellationToken);
    }

    /// <summary>
    /// Returns the hash as a lowercase hex string.
    /// </summary>
    /// <param name="directoryPath">
    /// Full or relative path to the directory.
    /// </param>
    /// <param name="cancellationToken">
    /// Operation cancellation token.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<string> ComputeChecksumBase64Async(string directoryPath, CancellationToken cancellationToken = default)
    {
        var hash = await this.ComputeChecksumAsync(directoryPath, cancellationToken).ConfigureAwait(false);

        return Convert.ToBase64String(hash);
    }

    private async Task<byte[]> ComputeDirectoryHashAsync(string directoryPath, CancellationToken cancellationToken)
    {
        var name = Path.GetFileName(directoryPath);
        if (string.IsNullOrEmpty(name))
        {
            name = directoryPath;
        }

        var nameBytes = Encoding.UTF8.GetBytes(name);

        var entries = Directory.GetFileSystemEntries(directoryPath).OrderBy(Path.GetFileName, StringComparer.Ordinal).ToArray();

        var childHashes = new List<byte[]>(entries.Length);

        foreach (var entry in entries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var childHash = Directory.Exists(entry) ? await this.ComputeDirectoryHashAsync(entry, cancellationToken) : await this.ComputeFileHashAsync(entry, cancellationToken);

            childHashes.Add(childHash);
        }

        var totalLength = nameBytes.Length + childHashes.Sum(h => h.Length);
        var buffer = new byte[totalLength];

        Buffer.BlockCopy(nameBytes, 0, buffer, 0, nameBytes.Length);

        var offset = nameBytes.Length;
        foreach (var hash in childHashes)
        {
            Buffer.BlockCopy(hash, 0, buffer, offset, hash.Length);
            offset += hash.Length;
        }

        return this.md5.ComputeHash(buffer);
    }

    private async Task<byte[]> ComputeFileHashAsync(string filePath, CancellationToken cancellationToken)
    {
        var nameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(filePath));

        await using var stream = File.OpenRead(filePath);
        using var memoryStream = new MemoryStream(nameBytes.Length + (int)stream.Length);

        memoryStream.Write(nameBytes);
        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return await this.md5.ComputeHashAsync(memoryStream, cancellationToken);
    }
}