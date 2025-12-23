// <copyright file="ArgumentsTests.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyChat.Test;

using MyChat;
using NUnit.Framework;

/// <summary>
/// Unit tests for <see cref="Arguments"/> parsing logic.
/// </summary>
public class ArgumentsTests
{
    /// <summary>
    /// Tests that parsing a single argument returns server mode.
    /// </summary>
    [Test]
    public void Arguments_Parse_SingleArgument_ReturnsServerMode()
    {
        var args = new[] { "5000" };
        var parsed = Arguments.Parse(args);

        Assert.Multiple(() =>
        {
            Assert.That(parsed.IsServer, Is.True);
            Assert.That(parsed.Port, Is.EqualTo(5000));
            Assert.That(parsed.IpAddress, Is.Null);
        });
    }

    /// <summary>
    /// Tests that parsing two arguments returns client mode.
    /// </summary>
    [Test]
    public void Arguments_Parse_TwoArguments_ReturnsClientMode()
    {
        var args = new[] { "127.0.0.1", "5000" };
        var parsed = Arguments.Parse(args);

        Assert.Multiple(() =>
        {
            Assert.That(parsed.IsServer, Is.False);
            Assert.That(parsed.Port, Is.EqualTo(5000));
            Assert.That(parsed.IpAddress, Is.EqualTo("127.0.0.1"));
        });
    }
}