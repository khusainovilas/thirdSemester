// <copyright file="RequestHandler.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyFTP;

using System.Net.Sockets;

/// <summary>
/// Processes a single client request using the MyFTP protocol.
/// Reads the command, executes a List or Get, and sends a response.
/// </summary>
internal class RequestHandler(NetworkStream stream)
{
    private readonly StreamReader reader = new(stream, leaveOpen: true);
    private readonly StreamWriter writer = new(stream, leaveOpen: true);

    /// <summary>
    /// Reads the query string, parses it, and calls List or Get.
    /// </summary>
    /// <returns>Response to the user.</returns>
    public async Task HandleAsync()
    {
        var request = await this.reader.ReadLineAsync();

        if (string.IsNullOrEmpty(request))
        {
            return;
        }

        var parts = request.Split(' ', 2);

        if (parts.Length < 2)
        {
            return;
        }

        var command = parts[0];
        var path = parts[1];

        switch (command)
        {
            // Type request - List
            case "1":
                await this.HandleListAsync(path);
                break;

            // Type request - Get
            case "2":
                await this.HandleGetAsync(path);
                break;

            // Unknown request type
            default:
                return;
        }
    }

    /// <summary>
    /// Converts a relative path to a secure full path within the server root.
    /// Returns null if the path is invalid or attempts to escape the root.
    /// </summary>
    /// <param name="relativePath">The path relative to the server root.</param>
    /// <returns>Secure full path or null.</returns>
    private static string? GetSecurePath(string relativePath)
    {
        try
        {
            var fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativePath));

            var root = Directory.GetCurrentDirectory();
            return !fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) ? null :
                fullPath;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Processes the List command.
    /// Returns a list of files and folders.
    /// </summary>
    /// <param name="relativePath">The path relative to the server root.</param>
    public async Task HandleListAsync(string relativePath)
    {
        var fullPath = GetSecurePath(relativePath);
        if (fullPath == null || !Directory.Exists(fullPath))
        {
            await this.SendErrorAsync(isList: true);
            return;
        }

        var entries = Directory.EnumerateFileSystemEntries(fullPath);
        var items = new List<string>();

        foreach (var entry in entries)
        {
            var name = Path.GetFileName(entry);
            var isDir = Directory.Exists(entry);
            items.Add($"{name} {isDir.ToString().ToLower()}");
        }

        var response = $"{items.Count}";
        if (items.Count > 0)
        {
            response += " " + string.Join(" ", items);
        }

        response += "\n";

        await this.writer.WriteLineAsync(response);
        await this.writer.FlushAsync();
    }

    /// <summary>
    /// Processes the Get command.
    /// Sends the file size and bytes.
    /// </summary>
    /// <param name="relativePath">The path relative to the server root.</param>
    public async Task HandleGetAsync(string relativePath)
    {
        var fullPath = GetSecurePath(relativePath);
        if (fullPath == null || !File.Exists(fullPath))
        {
            await this.SendErrorAsync(isList: false);
            return;
        }

        var fileBytes = await File.ReadAllBytesAsync(fullPath);
        long size = fileBytes.Length;

        await this.writer.WriteAsync($"{size} ");

        await this.writer.FlushAsync();
        await this.writer.BaseStream.WriteAsync(fileBytes);
        await this.writer.BaseStream.FlushAsync();
    }

    /// <summary>
    /// Sends an error response based on command type.
    /// </summary>
    /// <param name="isList">true for List (-1\n), false for Get (-1 ).</param>
    private async Task SendErrorAsync(bool isList)
    {
        if (isList)
        {
            await this.writer.WriteLineAsync("-1");
        }
        else
        {
            await this.writer.WriteAsync("-1 ");
        }

        await this.writer.FlushAsync();
    }
}