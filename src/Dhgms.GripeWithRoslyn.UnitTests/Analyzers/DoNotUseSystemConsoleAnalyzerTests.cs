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
    public sealed class DoNotUseSystemConsoleAnalyzerTests : CodeFixVerifier
    {
        //Diagnostic and CodeFix both triggered and checked for
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
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 18, 17)
                    }
            };

            VerifyCSharpDiagnostic(
                test,
                methodInvoke);
        }

        //protected override CodeFixProvider GetCSharpCodeFixProvider()
        //{
        //    return new DhgmsGripeWithRoslynAnalyzerCodeFixProvider();
        //}

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DoNotUseSystemConsoleAnalyzer();
        }
    }
}
