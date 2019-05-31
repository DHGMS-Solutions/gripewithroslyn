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
    public sealed class UseDateTimeUtcNowInsteadofNowAnalyzerTest : CodeFixVerifier
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
            public System.DateTime Get()
            {
                return System.DateTime.Now;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.UseDateTimeUtcNowInsteadofNow,
                Message = UseDateTimeUtcNowInsteadofNowAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 10, 24)
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
            return new UseDateTimeUtcNowInsteadofNowAnalyzer();
        }
    }
}
