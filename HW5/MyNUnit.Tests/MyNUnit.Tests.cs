// <copyright file="MyNUnit.Tests.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyNUnit.Tests;

using System;
using System.Linq;
using NUnitTest = NUnit.Framework.TestAttribute;

/// <summary>
/// Tests which check the correct behavior MyNUnit.
/// </summary>
public class MyNUnitRunnerTests
{
    private static readonly Type CalculatorTestsType = typeof(CalculatorTests);

    /// <summary>
    /// Test MyNUnit runs all tests.
    /// </summary>
    [NUnitTest]
    public void MyNUnitRunnerTests_CalculatorTests_Run_ShouldReturnCorrectStatistics()
    {
        var results = TestRunner.Run([CalculatorTestsType]);

        var passedCount = results.Count(r => r.Status == TestStatus.Passed);
        var failedCount = results.Count(r => r.Status == TestStatus.Failed);
        var ignoredCount = results.Count(r => r.Status == TestStatus.Ignored);

        Assert.Multiple(() =>
        {
            Assert.That(passedCount, Is.EqualTo(3), "Passed tests count is incorrect");
            Assert.That(failedCount, Is.EqualTo(1), "Failed tests count is incorrect");
            Assert.That(ignoredCount, Is.EqualTo(1), "Ignored tests count is incorrect");
        });
    }

    /// <summary>
    /// Test MyNUnit with an expected exception.
    /// </summary>
    [NUnitTest]
    public void MyNUnitRunnerTests_CalculatorTests_ExpectedException_ShouldPass()
    {
        var results = TestRunner.Run([CalculatorTestsType]);

        var testResult = results.First(
            r => r.TestName.EndsWith("Calculator_Divide_ByZero_ShouldThrow"));

        Assert.That(testResult.Status, Is.EqualTo(TestStatus.Passed));
    }

    /// <summary>
    /// Test MyNUnit ignored tests.
    /// </summary>
    [NUnitTest]
    public void MyNUnitRunnerTests_CalculatorTests_IgnoredTest_ShouldBeIgnored()
    {
        var results = TestRunner.Run([CalculatorTestsType]);

        var testResult = results.First(
            r => r.TestName.EndsWith("Calculator_Multiply_ShouldBeIgnored"));

        Assert.Multiple(() =>
        {
            Assert.That(testResult.Status, Is.EqualTo(TestStatus.Ignored));
            Assert.That(testResult.Message, Is.Not.Empty);
        });
    }

    /// <summary>
    /// Test MyNUnit a failing test.
    /// </summary>
    [NUnitTest]
    public void MyNUnitRunnerTests_CalculatorTests_FailedTest_ShouldFail()
    {
        var results = TestRunner.Run([CalculatorTestsType]);

        var testResult = results.First(
            r => r.TestName.EndsWith("Calculator_FailedTest_ShouldFail"));

        Assert.Multiple(() =>
        {
            Assert.That(testResult.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(testResult.Exception, Is.Not.Null);
        });
    }
}
