// <copyright file="ClassModel.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace Reflector;

/// <summary>
/// Represents a structural model of class.
/// </summary>
public class ClassModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClassModel"/> class.
    /// </summary>
    /// <param name="name">Class name.</param>
    /// <param name="namespaceName">Namespace of the class.</param>
    public ClassModel(string name, string namespaceName)
    {
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.Namespace = namespaceName;
        this.GenericParameters = new List<string>();
        this.ImplementedInterfaces = new List<string>();
        this.Fields = new List<FieldModel>();
        this.Methods = new List<MethodModel>();
        this.NestedClasses = new List<ClassModel>();
    }

    /// <summary>
    /// Gets the class name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the namespace of the class.
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// Gets or sets the accessibility level of the class.
    /// </summary>
    public Accessibility Accessibility { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the class is static.
    /// </summary>
    public bool IsStatic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the class is abstract.
    /// </summary>
    public bool IsAbstract { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the class is sealed.
    /// </summary>
    public bool IsSealed { get; set; }

    /// <summary>
    /// Gets or sets the base class name.
    /// </summary>
    public string? BaseClass { get; set; }

    /// <summary>
    /// Gets the generic parameters of the class.
    /// </summary>
    public IList<string> GenericParameters { get; }

    /// <summary>
    /// Gets the interfaces implemented by the class.
    /// </summary>
    public IList<string> ImplementedInterfaces { get; }

    /// <summary>
    /// Gets the fields declared in the class.
    /// </summary>
    public IList<FieldModel> Fields { get; }

    /// <summary>
    /// Gets the methods declared in the class.
    /// </summary>
    public IList<MethodModel> Methods { get; }

    /// <summary>
    /// Gets the nested classes declared in the class.
    /// </summary>
    public IList<ClassModel> NestedClasses { get; }
}
