// <copyright file="Program.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Sockets;
using System.Text;

const int port = 12345;
var root = Environment.CurrentDirectory;

var listener = new TcpListener(IPAddress.Any, port);
listener.Start();

Console.WriteLine($"Launched on the port {port}");
Console.WriteLine($"The root directory: {root}");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    _ = Task.Run(() => HandleClientAsync(client));
}

async Task HandleClientAsync(TcpClient client)
{
    Console.WriteLine("Successfully connected!");

    try
    {
        await using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true);

        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
            {
                break;
            }

            await ProcessRequestAsync(stream, line);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Client error: {ex.Message}");
    }
    finally
    {
        client.Dispose();
        Console.WriteLine("The client is disconnected");
    }
}

async Task ProcessRequestAsync(NetworkStream stream, string request)
{
    var parts = request.Split(' ', 2);
    if (parts.Length < 2 || !int.TryParse(parts[0], out var cmd))
    {
        return;
    }

    var path = parts[1];
    var fullPath = Path.GetFullPath(path);

    if (Path.GetRelativePath(root, fullPath).StartsWith("..", StringComparison.Ordinal))
    {
        await stream.WriteAsync("-1\n"u8.ToArray());
        return;
    }

    if (cmd == 1)
    {
        if (!Directory.Exists(fullPath))
        {
            await stream.WriteAsync("-1\n"u8.ToArray());
            return;
        }

        var entries = new DirectoryInfo(fullPath)
            .GetFileSystemInfos()
            .OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var sb = new StringBuilder();
        sb.Append(entries.Length);

        foreach (var e in entries)
        {
            var display = ToDisplayPath(e.FullName);
            var isDir = e is DirectoryInfo;
            sb.Append(' ').Append(display).Append(' ').Append(isDir ? "true" : "false");
        }

        sb.Append('\n');

        await stream.WriteAsync(Encoding.ASCII.GetBytes(sb.ToString()));
    }
    else if (cmd == 2)
    {
        if (!File.Exists(fullPath))
        {
            await stream.WriteAsync("-1\n"u8.ToArray());
            return;
        }

        var size = new FileInfo(fullPath).Length;

        await stream.WriteAsync(Encoding.ASCII.GetBytes(size.ToString()));

        await using var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true);
        await fs.CopyToAsync(stream);
    }
}

string ToDisplayPath(string fullPath)
{
    var rel = Path.GetRelativePath(root, fullPath).Replace('\\', '/');
    return rel == "." ? "." : rel.StartsWith("./") ? rel : "./" + rel;
}