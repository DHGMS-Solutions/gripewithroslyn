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
    public class DoNotUseSystemNetServicePointManagerAnalyzerTest : CodeFixVerifier
    {
        //Diagnostic and CodeFix both triggered and checked for
        [Fact]
        public void ReturnsWarningForMethodInvocation()
        {
            var test = @"
    namespace System.Net
    {
        public sealed class ServicePoint
        {
        }

        public sealed class System.Net.Security.RemoteCertificateValidationCallback
        {
        }

        public sealed class ServicePointManager
        {
            public static ServicePoint FindServicePoint(Uri uri)
            {
            }

            public static System.Net.Security.RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; }
        }
    }

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public void MethodName()
            {
                var servicePointManager = new System.Net.ServicePointManager();
                servicePointManager.FindServicePoint();
            }
        }
    }";
            var methodInvoke = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.DoNotUseSystemNetServicePointManager,
                Message = DoNotUseSystemNetServicePointManagerAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 29, 17)
                    }
            };

            VerifyCSharpDiagnostic(
                test,
                methodInvoke);
        }

        [Fact]
        public void ReturnsWarningForPropertyAccess()
        {
            var test = @"
    namespace System.Net
    {
        public sealed class ServicePoint
        {
        }

        public sealed class System.Net.Security.RemoteCertificateValidationCallback
        {
        }

        public sealed class ServicePointManager
        {
            public static ServicePoint FindServicePoint()
            {
            }

            public static System.Net.Security.RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; }
        }
    }

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public void MethodName()
            {
                var remoteCertificateValidationCallback = System.Net.ServicePointManager.RemoteCertificateValidationCallback;
            }
        }
    }";
            var methodInvoke = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.DoNotUseSystemNetServicePointManager,
                Message = DoNotUseSystemNetServicePointManagerAnalyzer.Title,
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
            return new DoNotUseSystemNetServicePointManagerAnalyzer();
        }
    }
}
