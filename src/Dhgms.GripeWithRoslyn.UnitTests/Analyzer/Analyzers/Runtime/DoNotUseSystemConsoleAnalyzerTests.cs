// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using CodeFixVerifier = Dhgms.GripeWithRoslyn.UnitTests.Verifiers.CodeFixVerifier;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzers.Runtime
{
    /// <summary>
    /// Unit Tests for <see cref="DoNotUseSystemConsoleAnalyzer"/>.
    /// </summary>
    public sealed class DoNotUseSystemConsoleAnalyzerTests : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning.
        /// </summary>
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace System
    {
        public sealed class Console
        {
            public static void Write(string value)
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
                System.Console.Write(""sometext"");
            }
        }
    }";
            var methodInvoke = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.DoNotUseSystemConsole,
                Message = DoNotUseSystemConsoleAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 18, 17)
                    }
            };

            VerifyCSharpDiagnostic(
                test,
                methodInvoke);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DoNotUseSystemConsoleAnalyzer();
        }
    }
}
