// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.XUnit;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using CodeFixVerifier = Dhgms.GripeWithRoslyn.UnitTests.Verifiers.CodeFixVerifier;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzers.XUnit
{
    /// <summary>
    /// Unit Tests for <see cref="DoNotUseXUnitInlineDataAttributeAnalyzerTest"/>.
    /// </summary>
    public class DoNotUseXUnitInlineDataAttributeAnalyzerTest : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning.
        /// </summary>
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace XUnit
    {
        [System.AttributeUsage(System.AttributeTargets.Method)]
        public sealed class InlineDataAttribute : System.Attribute
        {
        }
    }

    namespace ConsoleApplication1
    {
        using XUnit;

        public class TypeName
        {
            [InlineData()]
            public void MethodName()
            {
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.DoNotUseXUnitInlineDataAttribute,
                Message = "Do not use the XUnit Attribute InlineData",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 16, 14)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DoNotUseXUnitInlineDataAttributeAnalyzer();
        }
    }
}
