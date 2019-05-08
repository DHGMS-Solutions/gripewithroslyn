using Dhgms.GripeWithRoslyn.Analyzer.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace Dhgms.GripeWithRoslyn.Analyzer.UnitTests.Analyzers
{
    public sealed class DoNotUseGdiPlusAnalyzerTest : CodeFixVerifier
    {
        //Diagnostic and CodeFix both triggered and checked for
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
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 22, 30)
                    }
            };

            var methodInvoke = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.DoNotUseGdiPlus,
                Message = DoNotUseGdiPlusAnalyzer.Title,
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
            return new DoNotUseGdiPlusAnalyzer();
        }
    }
}