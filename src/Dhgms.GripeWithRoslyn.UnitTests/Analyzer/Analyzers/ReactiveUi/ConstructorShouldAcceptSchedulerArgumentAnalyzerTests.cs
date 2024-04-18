// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using CodeFixVerifier = Dhgms.GripeWithRoslyn.UnitTests.Verifiers.CodeFixVerifier;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.ReactiveUi
{
    /// <summary>
    /// Unit Tests for <see cref="ConstructorShouldAcceptSchedulerArgumentAnalyzer"/>.
    /// </summary>
    public sealed class ConstructorShouldAcceptSchedulerArgumentAnalyzerTests : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning.
        /// </summary>
        [Fact]
        public void ReturnsWarning()
        {
            const string test = @"
    namespace TestConsole
    {
        public class TestViewModel : ReactiveUI.ReactiveObject
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
                Id = DiagnosticIdsHelper.ConstructorShouldAcceptSchedulerArgument,
                Message = ConstructorShouldAcceptSchedulerArgumentAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 6, 13)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        /// <summary>
        /// Test to ensure good code doesn't return a warning.
        /// </summary>
        [Fact]
        public void ReturnsNoWarning()
        {
            const string test = @"
    namespace TestConsole
    {
        public class TestObject : ReactiveUI.ReactiveObject
        {
            public TestObject(System.Reactive.Concurrency.Scheduler scheduler)
            {
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConstructorShouldAcceptSchedulerArgumentAnalyzer();
        }
    }
}
