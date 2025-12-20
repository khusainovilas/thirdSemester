// <copyright file="Attributes.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Marks the method as a test.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the expected exception type.
    /// </summary>
    public Type? Expected { get; set; }
}

/// <summary>
/// Skip the test with an indication of the reason.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class IgnoreAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IgnoreAttribute"/> class.
    /// </summary>
    /// <param name="reason">The reason for ignoring the test.</param>
    public IgnoreAttribute(string reason)
    {
        this.Reason = reason ?? throw new ArgumentNullException(nameof(reason));
    }

    /// <summary>
    /// Gets the reason why the test is ignored.
    /// </summary>
    public string Reason { get; }
}

/// <summary>
/// Runs once before all tests in the class.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeClassAttribute : Attribute
{
}

/// <summary>
/// Runs once after all the tests in the class.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AfterClassAttribute : Attribute
{
}

/// <summary>
/// Runs before each test.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class BeforeAttribute : Attribute
{
}

/// <summary>
/// Runs after each test.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AfterAttribute : Attribute
{
}
