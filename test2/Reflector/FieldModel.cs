// <copyright file="FieldModel.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace Reflector;

/// <summary>
/// Represents a field declared in a class.
/// </summary>
public class FieldModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FieldModel"/> class.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <param name="typeName">The type of the field (e.g., int, string).</param>
    public FieldModel(string name, string typeName)
    {
        this.Name = name;
        this.TypeName = typeName;
    }

    /// <summary>
    /// Gets the name of the field.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of the field as a string.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets or sets the accessibility of the field (public, private, etc.).
    /// </summary>
    public Accessibility Accessibility { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field is static.
    /// </summary>
    public bool IsStatic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field is readonly.
    /// </summary>
    public bool IsReadonly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field is const.
    /// </summary>
    public bool IsConst { get; set; }
}
