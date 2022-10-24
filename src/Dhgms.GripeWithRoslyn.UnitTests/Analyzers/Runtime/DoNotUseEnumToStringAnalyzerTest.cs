// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using CodeFixVerifier = Dhgms.GripeWithRoslyn.UnitTests.Verifiers.CodeFixVerifier;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzers.Runtime
{
    /// <summary>
    /// Unit Tests for the GDI+ analyzer.
    /// </summary>
    public sealed class DoNotUseEnumToStringAnalyzerTest : CodeFixVerifier
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
        public enum SomeEnum
        {
            None,
            SomeValue
        }

        class TypeName
        {
            public void MethodName()
            {
                var name = SomeEnum.SomeValue.ToString();
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.DoNotUseEnumToString,
                Message = DoNotUseEnumToStringAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 28)
                    }
            };

            VerifyCSharpDiagnostic(
                test,
                expected);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DoNotUseEnumToStringAnalyzer();
        }
    }
}
