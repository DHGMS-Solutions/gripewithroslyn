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
    public sealed class DoNotUseGdiPlusAnalyzerTest : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning.
        /// </summary>
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace System.Drawing
    {
        public sealed class Bitmap
        {
            public Bitmap(string filename)
            {
            }

            public void Bleh()
            {
            }
        }
    }

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public void MethodName()
            {
                var bitmap = new System.Drawing.Bitmap(""somefile"");
                bitmap.Bleh();
            }
        }
    }";
            var ctor = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.DoNotUseGdiPlus,
                Message = DoNotUseGdiPlusAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 22, 30)
                    }
            };

            var methodInvoke = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.DoNotUseGdiPlus,
                Message = DoNotUseGdiPlusAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 23, 17)
                    }
            };

            VerifyCSharpDiagnostic(
                test,
                ctor,
                methodInvoke);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DoNotUseGdiPlusAnalyzer();
        }
    }
}