// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using CodeFixVerifier = Dhgms.GripeWithRoslyn.UnitTests.Verifiers.CodeFixVerifier;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzers.Language
{
    /// <summary>
    /// Unit Tests for checking if a Constructor invokes external methods.
    /// </summary>
    public sealed class ConstructorShouldNotInvokeExternalMethodsAnalyzerTests : CodeFixVerifier
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
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 17)
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

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConstructorShouldNotInvokeExternalMethodsAnalyzer();
        }
    }
}
