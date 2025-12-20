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
    /// Finds all test classes in the specified path.
    /// </summary>
    /// <param name="path">Path to directory or .dll file.</param>
    /// <returns>List of test class types.</returns>
    public static IReadOnlyList<Type> Discover(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var result = new List<Type>();

        foreach (var assembly in LoadAssemblies(path))
        {
            foreach (var type in assembly.GetTypes())
            {
                if (ContainsTests(type))
                {
                    result.Add(type);
                }
            }
        }

        return result;
    }

    private static bool ContainsTests(Type type)
    {
        return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Any(m => m.GetCustomAttribute<TestAttribute>() != null);
    }

    private static IEnumerable<Assembly> LoadAssemblies(string path)
    {
        if (Directory.Exists(path))
        {
            foreach (var file in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
            {
                var assembly = TryLoadAssembly(file);
                if (assembly != null)
                {
                    yield return assembly;
                }
            }
        }
        else if (File.Exists(path) && path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            var assembly = TryLoadAssembly(path);
            if (assembly != null)
            {
                yield return assembly;
            }
        }
        else
        {
            throw new FileNotFoundException($"Path not found: {path}");
        }
    }

    private static Assembly? TryLoadAssembly(string filePath)
    {
        try
        {
            return Assembly.LoadFrom(filePath);
        }
        catch (Exception ex) when (ex is BadImageFormatException or FileLoadException)
        {
            Console.WriteLine($"[Warning] Failed to load assembly: {filePath}");
            Console.WriteLine($"          {ex.Message}");
            return null;
        }
    }
}