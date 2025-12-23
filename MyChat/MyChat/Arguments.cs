// <copyright file="Arguments.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyChat;

using System;

/// <summary>
/// Represents parsed command-line arguments for the chat application.
/// </summary>
public class Arguments
{
    private Arguments(int port, string? ipAddress)
    {
        this.Port = port;
        this.IpAddress = ipAddress;
    }

    /// <summary>
    /// Gets the network port number.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Gets the IP address for client mode.
    /// </summary>
    public string? IpAddress { get; }

    /// <summary>
    /// Gets a value indicating whether returns the value, is it a server or an application.
    /// </summary>
    public bool IsServer => this.IpAddress is null;

    /// <summary>
    /// Parses command-line arguments.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>Parsed <see cref="Arguments"/> instance.</returns>
    public static Arguments Parse(string[] args)
    {
        if (args.Length == 1)
        {
            return new Arguments(int.Parse(args[0]), null);
        }

        if (args.Length == 2)
        {
            return new Arguments(int.Parse(args[1]), args[0]);
        }

        throw new ArgumentException("Invalid number of command-line arguments.");
    }
}
