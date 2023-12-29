// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Unit Tests for <see cref="MethodsThatUseReturnYieldShouldHaveNameThatBeginsWithEnumerateAnalyzer"/>.
    /// </summary>
    public sealed class MethodsThatUseReturnYieldShouldHaveNameThatBeginsWithEnumerateAnalyzerTests : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning.
        /// </summary>
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        using System.Text;

        public class TypeName
        {
            public IEnumerable<int> MethodName()
            {
                yield return 1;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.MethodsThatUseReturnYieldShouldHaveNameThatBeginsWithEnumerate,
                Message = MethodsThatUseReturnYieldShouldHaveNameThatBeginsWithEnumerateAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 37)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MethodsThatUseReturnYieldShouldHaveNameThatBeginsWithEnumerateAnalyzer();
        }
    }
}