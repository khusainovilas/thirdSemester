// <copyright file="Reflector.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace Reflector;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Provides simple methods for reflecting, generating, and comparing C# class structures.
/// </summary>
public static class Reflector
{
    /// <summary>
    /// Generates a C# file describing the given class type.
    /// </summary>
    /// <param name="someClass">The type to generate the structure from.</param>
    public static void PrintStructure(Type someClass)
    {
        ArgumentNullException.ThrowIfNull(someClass);

        var model = ClassModelBuilder.Build(someClass);
        var fileName = model.Name + ".cs";
        var text = string.Empty;
        
        if (!string.IsNullOrEmpty(model.Namespace))
        {
            text += "namespace " + model.Namespace + ";\n\n";
        }

        text += GetClassCode(model, 0);

        File.WriteAllText(fileName, text);
    }

    /// <summary>
    /// Compares two types and prints the differences between their fields and methods.
    /// </summary>
    /// <param name="a">The first type.</param>
    /// <param name="b">The second type.</param>
    public static void DiffClasses(Type a, Type b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var classA = ClassModelBuilder.Build(a);
        var classB = ClassModelBuilder.Build(b);

        Console.WriteLine("=== Comparing " + classA.Name + " and " + classB.Name + " ===\n");

        Console.WriteLine("--- Fields ---");
        foreach (var f in classA.Fields)
        {
            if (classB.Fields.All(fb => fb.Name != f.Name))
            {
                Console.WriteLine("Only in first: " + f.TypeName + " " + f.Name);
            }
        }

        foreach (var f in classB.Fields)
        {
            if (classA.Fields.All(fa => fa.Name != f.Name))
            {
                Console.WriteLine("Only in second: " + f.TypeName + " " + f.Name);
            }
        }

        foreach (var f in classA.Fields)
        {
            var fb = classB.Fields.FirstOrDefault(x => x.Name == f.Name);
            if (fb != null && !AreFieldsEqual(f, fb))
            {
                Console.WriteLine("Different field: " + f.Name + " first(" + f.TypeName + ") second(" + fb.TypeName + ")");
            }
        }

        Console.WriteLine("\n--- Methods ---");
        foreach (var m in classA.Methods)
        {
            if (!classB.Methods.Any(mb => mb.Name == m.Name && ParametersMatch(m.Parameters, mb.Parameters)))
            {
                Console.WriteLine("Only in first: " + m.Name + "()");
            }
        }

        foreach (var m in classB.Methods)
        {
            if (!classA.Methods.Any(ma => ma.Name == m.Name && ParametersMatch(ma.Parameters, m.Parameters)))
            {
                Console.WriteLine("Only in second: " + m.Name + "()");
            }
        }

        foreach (var m in classA.Methods)
        {
            var mb = classB.Methods.FirstOrDefault(x => x.Name == m.Name);
            if (mb != null && !AreMethodsEqual(m, mb))
            {
                Console.WriteLine("Different method: " + m.Name + "() first(" + m.ReturnTypeName + ") second(" + mb.ReturnTypeName + ")");
            }
        }

        Console.WriteLine("=== End of comparison ===\n");
    }

    private static string GetClassCode(ClassModel model, int indentLevel)
    {
        string indent = new(' ', indentLevel * 4);
        var code = string.Empty;

        var modifiers = GetAccessibilityString(model.Accessibility);
        if (model.IsStatic)
        {
            modifiers += " static";
        }
        else if (model.IsAbstract)
        {
            modifiers += " abstract";
        }
        else if (model.IsSealed)
        {
            modifiers += " sealed";
        }

        var generics = model.GenericParameters.Count > 0 ? "<" + string.Join(", ", model.GenericParameters) + ">" : string.Empty;
        var baseClass = !string.IsNullOrEmpty(model.BaseClass) ? " : " + model.BaseClass : string.Empty;

        code += indent + modifiers + " class " + model.Name + generics + baseClass + "\n";
        code += indent + "{\n";

        foreach (var field in model.Fields)
        {
            var fieldMod = GetAccessibilityString(field.Accessibility);
            if (field.IsStatic)
            {
                fieldMod += " static";
            }

            if (field.IsReadonly)
            {
                fieldMod += " readonly";
            }

            if (field.IsConst)
            {
                fieldMod += " const";
            }

            code += indent + "    " + fieldMod + " " + field.TypeName + " " + field.Name + ";\n";
        }

        foreach (var method in model.Methods)
        {
            var methodMod = GetAccessibilityString(method.Accessibility);
            if (method.IsStatic)
            {
                methodMod += " static";
            }

            if (method.IsAbstract)
            {
                methodMod += " abstract";
            }

            if (method.IsVirtual)
            {
                methodMod += " virtual";
            }

            if (method.IsOverride)
            {
                methodMod += " override";
            }

            var methodGenerics = method.GenericParameters.Count > 0 ? "<" + string.Join(", ", method.GenericParameters) + ">" : string.Empty;
            var parameters = string.Empty;
            for (var i = 0; i < method.Parameters.Count; i++)
            {
                var p = method.Parameters[i];
                if (!string.IsNullOrEmpty(p.Modifier))
                {
                    parameters += p.Modifier + " ";
                }

                parameters += p.TypeName + " " + p.Name;
                if (i < method.Parameters.Count - 1)
                {
                    parameters += ", ";
                }
            }

            code += indent + "    " + methodMod + " " + method.ReturnTypeName + " " + method.Name + methodGenerics + "(" + parameters + ")\n";
            code += indent + "    {\n";

            if (method.ReturnTypeName != "void")
            {
                code += indent + "        return " + GetDefaultValue(method.ReturnTypeName) + ";\n";
            }

            code += indent + "    }\n";
        }

        foreach (var nested in model.NestedClasses)
        {
            code += GetClassCode(nested, indentLevel + 1);
        }

        code += indent + "}\n\n";

        return code;
    }

    private static string GetAccessibilityString(Accessibility access)
    {
        return access switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            _ => "public",
        };
    }

    private static string GetDefaultValue(string typeName)
    {
        return typeName switch
        {
            "int" or "short" or "long" or "byte" or "sbyte" or "ushort" or "uint" or "ulong" or "float" or "double" or "decimal" => "0",
            "bool" => "false",
            "char" => "'\\0'",
            "string" => "string.Empty",
            _ => "null",
        };
    }

    private static bool AreFieldsEqual(FieldModel a, FieldModel b)
    {
        return a.TypeName == b.TypeName &&
               a.Accessibility == b.Accessibility &&
               a.IsStatic == b.IsStatic &&
               a.IsReadonly == b.IsReadonly &&
               a.IsConst == b.IsConst;
    }

    private static bool AreMethodsEqual(MethodModel a, MethodModel b)
    {
        if (a.ReturnTypeName != b.ReturnTypeName ||
            a.Accessibility != b.Accessibility ||
            a.IsStatic != b.IsStatic ||
            a.IsAbstract != b.IsAbstract ||
            a.IsVirtual != b.IsVirtual ||
            a.IsOverride != b.IsOverride ||
            a.GenericParameters.Count != b.GenericParameters.Count ||
            a.Parameters.Count != b.Parameters.Count)
        {
            return false;
        }

        for (var i = 0; i < a.Parameters.Count; i++)
        {
            if (a.Parameters[i].TypeName != b.Parameters[i].TypeName ||
                a.Parameters[i].Name != b.Parameters[i].Name ||
                a.Parameters[i].Modifier != b.Parameters[i].Modifier)
            {
                return false;
            }
        }

        for (var i = 0; i < a.GenericParameters.Count; i++)
        {
            if (a.GenericParameters[i] != b.GenericParameters[i])
            {
                return false;
            }
        }

        return true;
    }

    private static bool ParametersMatch(List<ParameterModel> a, List<ParameterModel> b)
    {
        if (a.Count != b.Count)
        {
            return false;
        }

        for (var i = 0; i < a.Count; i++)
        {
            if (a[i].TypeName != b[i].TypeName)
            {
                return false;
            }
        }

        return true;
    }
}
