// <copyright file="ChatRunner.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyChat;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

/// <summary>
/// Provides methods for running a chat application in server or client mode.
/// </summary>
public static class ChatRunner
{
    private static readonly List<TcpClient> ConnectedClients = [];

    /// <summary>
    /// Starts the chat application based on the provided arguments.
    /// </summary>
    /// <param name="arguments">Arguments containing port and optional IP address.</param>
    /// <returns>A <see cref="Task"/> representing the async operation.</returns>
    public static async Task RunAsync(Arguments arguments)
    {
        if (arguments.IsServer)
        {
            await RunServerAsync(arguments.Port);
        }
        else
        {
            await RunClientAsync(arguments.IpAddress!, arguments.Port);
        }
    }

    /// <summary>
    /// Runs the chat application as a server listening.
    /// </summary>
    /// <param name="port">Port number to listen on.</param>
    /// <returns>Representing the asynchronous server operation.</returns>
    public static async Task RunServerAsync(int port)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Server listening on port {port}...");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            lock (ConnectedClients)
            {
                ConnectedClients.Add(client);
            }

            var endpoint = client.Client.RemoteEndPoint?.ToString() ?? "Unknown";

            Console.WriteLine($"Client connected: {endpoint}");

            _ = Task.Run(async () =>
            {
                await RunChatAsync(client, endpoint);

                Console.WriteLine($"Client disconnected: {endpoint}");
                lock (ConnectedClients)
                {
                    ConnectedClients.Remove(client);
                }

                lock (ConnectedClients)
                {
                    if (ConnectedClients.Count == 0)
                    {
                        Console.WriteLine("No clients connected. Server shutting down.");
                        Environment.Exit(0);
                    }
                }
            });
        }
    }

    /// <summary>
    /// Runs the chat application as a client.
    /// </summary>
    /// <param name="ip">IP address of the server to connect to.</param>
    /// <param name="port">Port number of the server.</param>
    /// <returns>Representing the async client operation.</returns>
    public static async Task RunClientAsync(string ip, int port)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(ip, port);
        Console.WriteLine($"Connected to server {ip}:{port}");
        await RunChatAsync(client, "Server");
    }

    private static async Task RunChatAsync(TcpClient client, string name)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        var consoleTask = Task.Run(async () =>
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null || line.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                await writer.WriteLineAsync(line);
            }
        });

        var networkTask = Task.Run(async () =>
        {
            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }

                Console.WriteLine($"> {line}");
            }
        });

        await Task.WhenAny(consoleTask, networkTask);
        client.Close();
    }
}
