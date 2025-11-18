// <copyright file="Program.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

using System.Net.Sockets;
using System.Text;

const string host = "127.0.0.1";
const int port = 12345;

TcpClient? client = null;

try
{
    client = new TcpClient();
    await client.ConnectAsync(host, port);
    var stream = client.GetStream();

    while (true)
    {
        Console.Write("\n> ");
        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input))
        {
            continue;
        }

        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        var parts = input.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2 || (parts[0] != "1" && parts[0] != "2"))
        {
            Console.WriteLine(" 1 <path> - list | 2 <path>  – get | exit ");
            continue;
        }

        await stream.WriteAsync(Encoding.ASCII.GetBytes(input + "\n"));

        if (parts[0] == "1")
        {
            var line = await ReadLineAsync(stream);
            Console.WriteLine(line ?? "-1");
        }
        else
        {
            var (size, firstByte) = await ReadSizeAndFirstByteAsync(stream);

            if (size == -1)
            {
                Console.WriteLine("-1");
                continue;
            }

            if (size < 0)
            {
                throw new FormatException($"Invalid file size received: {size}");
            }

            Console.Write(size);
            Console.Write(' ');

            var fileName = Path.GetFileName(parts[1]);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "file";
            }

            await using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);

            var remaining = size;
            const int previewBytes = 256;
            var previewLeft = Math.Min(previewBytes, size);
            var buffer = new byte[81920];

            if (firstByte.HasValue)
            {
                if (previewLeft > 0)
                {
                    Console.OpenStandardOutput().WriteByte(firstByte.Value);
                    previewLeft--;
                }

                await fs.WriteAsync(new[] { firstByte.Value });
                remaining--;
            }

            while (remaining > 0)
            {
                var toRead = (int)Math.Min(buffer.Length, remaining);
                var read = await stream.ReadAsync(buffer.AsMemory(0, toRead));
                if (read <= 0)
                {
                    throw new IOException("Connection lost");
                }

                if (previewLeft > 0)
                {
                    var previewThisTime = (int)Math.Min(read, previewLeft);
                    Console.OpenStandardOutput().Write(buffer, 0, previewThisTime);
                    previewLeft -= previewThisTime;
                }

                await fs.WriteAsync(buffer.AsMemory(0, read));
                remaining -= read;
            }

            if (size > previewBytes)
            {
                Console.WriteLine($"\n... [truncated, full file saved as {fileName}]");
            }
            else
            {
                Console.WriteLine();
            }
        }
    }
}
catch (SocketException) when (client is { Connected: false })
{
    Console.Error.WriteLine("Error: Could not connect to server");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.GetType().Name}: {ex.Message}");
}
finally
{
    client?.Dispose();
}

return;

static async Task<string?> ReadLineAsync(NetworkStream s)
{
    var sb = new StringBuilder();
    var b = new byte[1];

    while (await s.ReadAsync(b) > 0)
    {
        if (b[0] == '\n')
        {
            break;
        }

        if (b[0] != '\r')
        {
            sb.Append((char)b[0]);
        }
    }

    return sb.Length == 0 ? null : sb.ToString();
}

static async Task<(long Size, byte? FirstByte)> ReadSizeAndFirstByteAsync(NetworkStream s)
{
    var sb = new StringBuilder();
    var b = new byte[1];

    while (await s.ReadAsync(b) > 0)
    {
        var ch = (char)b[0];

        if (ch is >= '0' and <= '9')
        {
            sb.Append(ch);
        }
        else if (ch == '-' && sb.Length == 0)
        {
            sb.Append(ch);
        }
        else
        {
            var sizeStr = sb.ToString();

            if (sizeStr == "-1")
            {
                return (-1, null);
            }

            if (!long.TryParse(sizeStr, out var size) || size < 0)
            {
                throw new FormatException($"Invalid size format from server: '{sizeStr}'");
            }

            return (size, b[0]);
        }
    }

    throw new IOException("Unexpected end of stream while reading file size");
}