using System;
using System.Collections.Generic;
using System.Text;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace Dhgms.GripeWithRoslyn.Analyzer.UnitTests.Analyzers
{
    public class StructureMapShouldNotBeUsedAnalyzerTest : CodeFixVerifier
    {
        //Diagnostic and CodeFix both triggered and checked for
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
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 22, 32)
                    }
            };

            var methodInvoke = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.StructureMapShouldNotBeUsed,
                Message = "StructureMap is end of life so should not be used.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 23, 17)
                    }
            };

            VerifyCSharpDiagnostic(
                test,
                ctor,
                methodInvoke);
        }

        //protected override CodeFixProvider GetCSharpCodeFixProvider()
        //{
        //    return new DhgmsGripeWithRoslynAnalyzerCodeFixProvider();
        //}

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new StructureMapShouldNotBeUsedAnalyzer();
        }
    }
}
