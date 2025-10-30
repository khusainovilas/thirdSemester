// <copyright file="Program.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Sockets;
using MyFTP;

const int port = 9091;

var listener = new TcpListener(IPAddress.Any, port);
listener.Start();

Console.WriteLine($"MyFTP server running on port {port}");
Console.WriteLine($"Root directory: {Directory.GetCurrentDirectory()}");
Console.WriteLine("Waiting for clients...");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    _ = Task.Run(async () =>
    {
        using (client)
        await using (var stream = client.GetStream())
        {
            var handler = new RequestHandler(stream);

            try
            {
                while (true)
                {
                    var request = await new StreamReader(stream, leaveOpen: true).ReadLineAsync();

                    if (string.IsNullOrEmpty(request))
                    {
                        break;
                    }

                    if (request.Trim().Equals("exit", StringComparison.CurrentCultureIgnoreCase))
                    {
                        break;
                    }

                    var parts = request.Split(' ', 2);
                    if (parts.Length < 2)
                    {
                        continue;
                    }

                    switch (parts[0])
                    {
                        case "1":
                            await handler.HandleListAsync(parts[1]);
                            break;
                        case "2":
                            await handler.HandleGetAsync(parts[1]);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
        }

        Console.WriteLine("Client disconnected.");
    });
}