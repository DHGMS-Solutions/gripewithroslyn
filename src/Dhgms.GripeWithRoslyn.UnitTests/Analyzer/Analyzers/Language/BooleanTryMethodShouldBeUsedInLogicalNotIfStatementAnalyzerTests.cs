// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using CodeFixVerifier = Dhgms.GripeWithRoslyn.UnitTests.Verifiers.CodeFixVerifier;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzers.Language
{
    /// <summary>
    /// Unit Tests for <see cref="BooleanTryMethodShouldBeUsedInLogicalNotIfStatementAnalyzer"/>.
    /// </summary>
    public sealed class BooleanTryMethodShouldBeUsedInLogicalNotIfStatementAnalyzerTests : CodeFixVerifier
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

        class TypeName
        {
            public void MethodName()
            {
                int.TryParse(""x"", out var result);
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.TryParseShouldBeUsedInLogicalNotIfStatement,
                Message = BooleanTryMethodShouldBeUsedInLogicalNotIfStatementAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new BooleanTryMethodShouldBeUsedInLogicalNotIfStatementAnalyzer();
        }
    }
}
