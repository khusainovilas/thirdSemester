// <copyright file="TestDiscovery.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNUnit;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

/// <summary>
/// Searches for test classes and test methods in assemblies.
/// </summary>
public static class TestDiscovery
{
    /// <summary>
    /// Discovers test classes in a DLL.
    /// </summary>
    /// <param name="dllPath">Path to test assembly.</param>
    /// <returns>List of test class types.</returns>
    public static IReadOnlyList<Type> DiscoverFromDll(string dllPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dllPath);

        if (!File.Exists(dllPath))
        {
            throw new FileNotFoundException("DLL not found", dllPath);
        }

        if (!dllPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("File must be a DLL", nameof(dllPath));
        }

        var assembly = Assembly.LoadFrom(dllPath);

        return assembly
            .GetTypes()
            .Where(ContainsTests)
            .ToList();
    }

    private static bool ContainsTests(Type type)
    {
        return type.GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic)
            .Any(m => m.GetCustomAttribute<TestAttribute>() != null);
    }
}