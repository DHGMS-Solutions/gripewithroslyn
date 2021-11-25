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
    public sealed class DoNotUseSystemSecuritySecureStringAnalyzerTest : CodeFixVerifier
    {
        //Diagnostic and CodeFix both triggered and checked for
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace System.Security
    {
        public sealed class SecureString
        {
            public void Clear()
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
                var secureString = new System.Security.SecureString();
                secureString.Clear();
            }
        }
    }";
            var methodInvoke = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.DoNotUseSystemSecuritySecureString,
                Message = DoNotUseSystemSecuritySecureStringAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 19, 17)
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
            return new DoNotUseSystemSecuritySecureStringAnalyzer();
        }
    }
}
