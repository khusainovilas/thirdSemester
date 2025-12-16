// <copyright file="ReflectorTests.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace Reflector.Test;

public class SampleClass
{
    public int Number;
    private string Text;

    public void SayHello() { }
    public int GetNumber() => Number;
}

public class SampleClass2
{
    public int Number;
    public string Text;

    public void SayHello() { }
    public int GetNumber() => Number;
    public void ExtraMethod() { }
}

[TestFixture]
public class ReflectorOutput
{
    [Test]
    public void ShowPrintStructure()
    {
        Reflector.PrintStructure(typeof(SampleClass));
    }

    [Test]
    public void ShowDiffClasses()
    {
        Reflector.DiffClasses(typeof(SampleClass), typeof(SampleClass2));
    }
}