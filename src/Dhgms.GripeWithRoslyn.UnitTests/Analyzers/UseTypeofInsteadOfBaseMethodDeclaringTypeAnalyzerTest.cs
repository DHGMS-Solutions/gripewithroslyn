using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace Dhgms.GripeWithRoslyn.Analyzer.UnitTests.Analyzers
{
    public sealed class UseTypeofInsteadOfBaseMethodDeclaringTypeAnalyzerTest : CodeFixVerifier
    {
        //Diagnostic and CodeFix both triggered and checked for
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
                global::System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            }
        }
    }";

            var a = MethodBase.GetCurrentMethod().DeclaringType;

            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.UseEncodingUnicodeInsteadOfASCII,
                Message = "Consider usage of typeof(x) instead of MethodBase.GetCurrentMethod().DeclaringType.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 10, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        //protected override CodeFixProvider GetCSharpCodeFixProvider()
        //{
        //    return new DhgmsGripeWithRoslynAnalyzerCodeFixProvider();
        //}

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new UseTypeofInsteadOfBaseMethodDeclaringTypeAnalyzer();
        }
    }
}
