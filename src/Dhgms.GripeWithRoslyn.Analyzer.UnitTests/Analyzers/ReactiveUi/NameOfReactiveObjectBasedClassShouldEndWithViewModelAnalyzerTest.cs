﻿using System;
using System.Collections.Generic;
using System.Text;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace Dhgms.GripeWithRoslyn.Analyzer.UnitTests.Analyzers.ReactiveUi
{
    public sealed class NameOfReactiveObjectBasedClassShouldEndWithViewModelAnalyzerTest : CodeFixVerifier
    {
        [Fact]
        public void ReturnsWarning()
        {
            const string test = @"
    namespace ReactiveUi
    {
        public class ReactiveObject
        {
        }

        public class Test : ReactiveUi.ReactiveObject
        {
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.ReactiveObjectClassShouldHaveViewModelSuffix,
                Message = NameOfReactiveObjectBasedClassShouldEndWithViewModelAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 8, 22)
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
            return new NameOfReactiveObjectBasedClassShouldEndWithViewModelAnalyzer();
        }
    }
}
