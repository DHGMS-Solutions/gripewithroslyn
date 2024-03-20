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
    /// Unit Tests for <see cref="DoNotUseObjectAsFieldTypeAnalyzer"/>.
    /// </summary>
    public sealed class DoNotUseObjectAsFieldTypeAnalyzerTests : CodeFixVerifier
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
            private readonly object _field = new object();
            private readonly System.Object _field = new System.Object();
        }
    }";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.DoNotUseObjectAsFieldType,
                    Message = DoNotUseObjectAsFieldTypeAnalyzer.Title,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                    [
                        new DiagnosticResultLocation("Test0.cs", 6, 30)
                    ]
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.DoNotUseObjectAsFieldType,
                    Message = DoNotUseObjectAsFieldTypeAnalyzer.Title,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                    [
                        new DiagnosticResultLocation("Test0.cs", 7, 30)
                    ]
                },
            };

            VerifyCSharpDiagnostic(
                test,
                expected);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DoNotUseObjectAsFieldTypeAnalyzer();
        }
    }
}
