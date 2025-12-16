// <copyright file="ParameterModel.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace Reflector;

/// <summary>
/// Represents a parameter of a method.
/// </summary>
public class ParameterModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterModel"/> class.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="typeName">The type of the parameter (e.g., int, string).</param>
    /// <param name="modifier">The modifier of the parameter (ref, out, in) or empty string if none.</param>
    public ParameterModel(string name, string typeName, string modifier = "")
    {
        this.Name = name;
        this.TypeName = typeName;
        this.Modifier = modifier;
    }

    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of the parameter as a string.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets or sets the modifier of the parameter.
    /// </summary>
    public string Modifier { get; set; }
}
