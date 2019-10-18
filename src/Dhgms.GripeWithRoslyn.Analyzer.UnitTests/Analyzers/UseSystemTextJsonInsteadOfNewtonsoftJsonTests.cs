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
    public sealed class UseSystemTextJsonInsteadOfNewtonsoftJsonTests : CodeFixVerifier
    {
        //Diagnostic and CodeFix both triggered and checked for
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace Newtonsoft.Json
    {
        public static class JsonConvert
        {
            public static object DeserializeObject(string value)
            {
                return value;
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
                global::Newtonsoft.Json.JsonConvert.DeserializeObject(""{}"");
            }
        }
    }";

            var a = MethodBase.GetCurrentMethod().DeclaringType;

            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.UseSystemTextJsonInsteadOfNewtonsoftJson,
                Message = UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 21, 17)
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
            return new UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer();
        }
    }

}
