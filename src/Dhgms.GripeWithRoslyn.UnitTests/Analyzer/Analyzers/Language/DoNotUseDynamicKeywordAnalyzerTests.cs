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
    /// Unit Tests for <see cref="DoNotUseDynamicAsParameterTypeAnalyzer"/>.
    /// </summary>
    public sealed class DoNotUseDynamicKeywordAnalyzerTests : CodeFixVerifier
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
            public void MethodName(dynamic arg)
            {
            }
        }
    }";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.DoNotUseDynamicAsParameterType,
                    Message = DoNotUseDynamicAsParameterTypeAnalyzer.Title,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                    [
                        new DiagnosticResultLocation("Test0.cs", 6, 36)
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
            return new DoNotUseDynamicAsParameterTypeAnalyzer();
        }
    }
}
