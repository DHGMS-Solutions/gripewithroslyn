// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.StructureMap;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using CodeFixVerifier = Dhgms.GripeWithRoslyn.UnitTests.Verifiers.CodeFixVerifier;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzers.StructureMap
{
    /// <summary>
    /// Unit Tests for <see cref="StructureMapShouldNotBeUsedAnalyzer"/>.
    /// </summary>
    public class StructureMapShouldNotBeUsedAnalyzerTest : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning.
        /// </summary>
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace StructureMap
    {
        public class Test
        {
            public void Method()
            {
            }
        }
    }

    namespace ConsoleApplication1
    {
        using System.Text;



        class TypeName
        {
            public void MethodName()
            {
                var instance = new StructureMap.Test();
                instance.Method();
            }
        }
    }";

            var ctor = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.StructureMapShouldNotBeUsed,
                Message = "StructureMap is end of life so should not be used.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 22, 32)
                    }
            };

            var methodInvoke = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.StructureMapShouldNotBeUsed,
                Message = "StructureMap is end of life so should not be used.",
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
            return new StructureMapShouldNotBeUsedAnalyzer();
        }
    }
}
