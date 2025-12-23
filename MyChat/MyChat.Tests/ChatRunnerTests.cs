// <copyright file="ChatRunnerTests.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyChat.Test;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MyChat;
using NUnit.Framework;

/// <summary>
/// Tests for <see cref="ChatRunner"/>.
/// </summary>
[TestFixture]
public class ChatRunnerTests
{
    /// <summary>
    /// Tests that a server and client can exchange a single message.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ChatRunner_RunAsync_ServerReceive()
    {
        const string ipAddress = "127.0.0.1";
        const int port = 5005;

        string serverReceived = string.Empty;
        string clientMessage = "Hello!";

        var serverTask = Task.Run(async () =>
        {
            var listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            listener.Start();

            using var client = await listener.AcceptTcpClientAsync();
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream) { AutoFlush = true };

            serverReceived = await reader.ReadLineAsync() ?? string.Empty;
            await writer.WriteLineAsync(serverReceived);

            listener.Stop();
        });

        var clientTask = Task.Run(async () =>
        {
            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Parse(ipAddress), port);
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream) { AutoFlush = true };

            await writer.WriteLineAsync(clientMessage);
            var received = await reader.ReadLineAsync()!;
            return received;
        });

        var clientReceived = await clientTask;
        await serverTask;

        Assert.Multiple(() =>
        {
            Assert.That(serverReceived, Is.EqualTo(clientMessage));
            Assert.That(clientReceived, Is.EqualTo(clientMessage));
        });
    }
}
