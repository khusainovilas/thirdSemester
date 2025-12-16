// <copyright file="ClassModelBuilder.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace Reflector;

using System;
using System.Reflection;

/// <summary>
/// Builds a ClassModel using reflection.
/// </summary>
public static class ClassModelBuilder
{
    /// <summary>
    /// Builds a <see cref="ClassModel"/> from a given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The type to analyze.</param>
    /// <returns>A <see cref="ClassModel"/> representing the type.</returns>
    public static ClassModel Build(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var classModel = new ClassModel(type.Name, type.Namespace ?? string.Empty)
        {
            Accessibility = GetAccessibility(type),
            IsAbstract = type.IsAbstract && !type.IsInterface,
            IsSealed = type.IsSealed,
            IsStatic = type.IsAbstract && type.IsSealed,
            BaseClass = type.BaseType != null && type.BaseType != typeof(object) ? type.BaseType.Name : null,
        };

        foreach (var generic in type.GetGenericArguments())
        {
            classModel.GenericParameters.Add(generic.Name);
        }

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            classModel.Fields.Add(new FieldModel(field.Name, field.FieldType.Name)
            {
                Accessibility = GetAccessibility(field),
                IsStatic = field.IsStatic,
                IsReadonly = field.IsInitOnly,
                IsConst = field.IsLiteral,
            });
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (method.IsSpecialName)
            {
                continue;
            }

            var methodModel = new MethodModel(method.Name, method.ReturnType.Name)
            {
                Accessibility = GetAccessibility(method),
                IsStatic = method.IsStatic,
                IsAbstract = method.IsAbstract,
                IsVirtual = method.IsVirtual && !method.IsAbstract,
                IsOverride = method.GetBaseDefinition().DeclaringType != method.DeclaringType,
            };

            foreach (var gen in method.GetGenericArguments())
            {
                methodModel.GenericParameters.Add(gen.Name);
            }

            foreach (var param in method.GetParameters())
            {
                var modifier = string.Empty;
                if (param.IsOut)
                {
                    modifier = "out";
                }
                else if (param.ParameterType.IsByRef)
                {
                    modifier = "ref";
                }

                methodModel.Parameters.Add(new ParameterModel(param.Name, param.ParameterType.Name, modifier));
            }

            classModel.Methods.Add(methodModel);
        }

        foreach (var nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
        {
            classModel.NestedClasses.Add(Build(nested));
        }

        return classModel;
    }

    /// <summary>
    /// Return the accessibility of a type.
    /// </summary>
    private static Accessibility GetAccessibility(Type type)
    {
        if (type.IsPublic || type.IsNestedPublic)
        {
            return Accessibility.Public;
        }

        if (type.IsNestedFamily)
        {
            return Accessibility.Protected;
        }

        if (type.IsNestedAssembly)
        {
            return Accessibility.Internal;
        }

        if (type.IsNestedPrivate)
        {
            return Accessibility.Private;
        }

        return Accessibility.Public;
    }

    /// <summary>
    /// Return the accessibility of a field.
    /// </summary>
    private static Accessibility GetAccessibility(FieldInfo field)
    {
        if (field.IsPublic)
        {
            return Accessibility.Public;
        }

        if (field.IsFamily)
        {
            return Accessibility.Protected;
        }

        if (field.IsAssembly)
        {
            return Accessibility.Internal;
        }

        if (field.IsPrivate)
        {
            return Accessibility.Private;
        }

        return Accessibility.Public;
    }

    /// <summary>
    /// Return the accessibility of a method.
    /// </summary>
    private static Accessibility GetAccessibility(MethodInfo method)
    {
        if (method.IsPublic)
        {
            return Accessibility.Public;
        }

        if (method.IsFamily)
        {
            return Accessibility.Protected;
        }

        if (method.IsAssembly)
        {
            return Accessibility.Internal;
        }

        if (method.IsPrivate)
        {
            return Accessibility.Private;
        }

        return Accessibility.Public;
    }
}
