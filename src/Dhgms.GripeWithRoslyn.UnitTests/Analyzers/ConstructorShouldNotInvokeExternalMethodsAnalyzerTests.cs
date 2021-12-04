using System;
using System.Collections.Generic;
using System.Text;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace Dhgms.GripeWithRoslyn.Analyzer.UnitTests.Analyzers
{
    public sealed class ConstructorShouldNotInvokeExternalMethodsAnalyzerTests : CodeFixVerifier
    {
        [Fact]
        public void ReturnsWarning()
        {
            const string test = @"
    namespace TestConsole
    {
        public class TestObject
        {
            public TestObject()
            {
                SomeMethod();
            }

            private static void SomeMethod()
            {
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.ConstructorShouldNotInvokeExternalMethods,
                Message = ConstructorShouldNotInvokeExternalMethodsAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 8, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void ReturnsNoWarning()
        {
            const string test = @"
    namespace TestConsole
    {
        public class TestObject
        {
            private readonly int _someField = SomeMethod2();

            public TestObject(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }
            }

            private int Property
            {
                get
                {
                    // horrid test
                    return SomeMethod2();
                }
            }

            private static int SomeMethod()
            {
                return SomeMethod2();
            }

            private static int SomeMethod2()
            {
                return 0;
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        //protected override CodeFixProvider GetCSharpCodeFixProvider()
        //{
        //    return new DhgmsGripeWithRoslynAnalyzerCodeFixProvider();
        //}

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConstructorShouldNotInvokeExternalMethodsAnalyzer();
        }
    }
}
