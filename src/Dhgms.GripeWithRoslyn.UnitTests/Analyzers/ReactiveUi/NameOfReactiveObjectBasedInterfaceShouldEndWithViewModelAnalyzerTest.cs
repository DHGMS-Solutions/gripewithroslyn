using System;
using System.Collections.Generic;
using System.Text;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace Dhgms.GripeWithRoslyn.Analyzer.UnitTests.Analyzers.ReactiveUi
{
    public sealed class NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzerTest : CodeFixVerifier
    {
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace ReactiveUi
    {
        public interface IReactiveObject
        {
        }

        public interface Test : ReactiveUi.IReactiveObject
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.ReactiveObjectInterfaceShouldHaveViewModelSuffix,
                Message = NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 8, 26)
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
            return new NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzer();
        }
    }
}
