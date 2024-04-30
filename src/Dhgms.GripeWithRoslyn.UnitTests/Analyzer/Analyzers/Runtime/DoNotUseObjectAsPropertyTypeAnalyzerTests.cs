// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.Runtime
{
    /// <summary>
    /// Unit Tests for <see cref="DoNotUseObjectAsPropertyTypeAnalyzer"/>.
    /// </summary>
    public sealed class DoNotUseObjectAsPropertyTypeAnalyzerTests : CodeFixVerifier
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
        class TypeName
        {
            public object SomeProperty => new object();

            public System.Object SomeProperty2 => new System.Object();
        }
    }";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.DoNotUseObjectAsPropertyType,
                    Message = DoNotUseObjectAsPropertyTypeAnalyzer.Title,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                    [
                        new DiagnosticResultLocation("Test0.cs", 6, 20)
                    ]
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.DoNotUseObjectAsPropertyType,
                    Message = DoNotUseObjectAsPropertyTypeAnalyzer.Title,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                    [
                        new DiagnosticResultLocation("Test0.cs", 8, 20)
                    ]
                }
            };

            VerifyCSharpDiagnostic(
                test,
                expected);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DoNotUseObjectAsPropertyTypeAnalyzer();
        }
    }
}
