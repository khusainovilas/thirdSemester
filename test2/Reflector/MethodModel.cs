// <copyright file="MethodModel.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace Reflector;

/// <summary>
/// Represents a method declared in a class.
/// </summary>
public class MethodModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodModel"/> class.
    /// </summary>
    /// <param name="name">The name of the method.</param>
    /// <param name="returnTypeName">The return type of the method (e.g., void, int).</param>
    public MethodModel(string name, string returnTypeName)
    {
        this.Name = name;
        this.ReturnTypeName = returnTypeName;
        this.Parameters = new List<ParameterModel>();
        this.GenericParameters = new List<string>();
    }

    /// <summary>
    /// Gets the name of the method.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the return type of the method as a string.
    /// </summary>
    public string ReturnTypeName { get; }

    /// <summary>
    /// Gets or sets the accessibility of the method (public, private, etc.).
    /// </summary>
    public Accessibility Accessibility { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the method is static.
    /// </summary>
    public bool IsStatic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the method is abstract.
    /// </summary>
    public bool IsAbstract { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the method is virtual.
    /// </summary>
    public bool IsVirtual { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the method overrides a base method.
    /// </summary>
    public bool IsOverride { get; set; }

    /// <summary>
    /// Gets the generic type parameters of the method.
    /// </summary>
    public List<string> GenericParameters { get; }

    /// <summary>
    /// Gets the parameters of the method.
    /// </summary>
    public List<ParameterModel> Parameters { get; }
}
