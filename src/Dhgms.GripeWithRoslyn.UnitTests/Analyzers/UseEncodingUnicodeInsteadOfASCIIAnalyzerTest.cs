using System;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace Dhgms.GripeWithRoslyn.Analyzer.Test.Analyzers
{
    public class UseEncodingUnicodeInsteadOfASCIIAnalyzerTest : CodeFixVerifier
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
                char[] testValue = null;
                ASCIIEncoding.GetBytes(testValue, 0, 0);
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.UseEncodingUnicodeInsteadOfASCII,
                Message = "Consider usage of Unicode Encoding instead of ASCII.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 11, 17)
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
            return new UseEncodingUnicodeInsteadOfASCIIAnalyzer();
        }
    }
}