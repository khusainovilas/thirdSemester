// <copyright file="Program.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

using MyChat;

try
{
    var arguments = Arguments.Parse(args);
    await ChatRunner.RunAsync(arguments);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
