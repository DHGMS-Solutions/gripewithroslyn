// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using CodeFixVerifier = Dhgms.GripeWithRoslyn.UnitTests.Verifiers.CodeFixVerifier;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzers.ReactiveUi
{
    /// <summary>
    /// Unit Tests for <see cref="NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzer"/>.
    /// </summary>
    public sealed class NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzerTest : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning.
        /// </summary>
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace ReactiveUi
    {
        public interface IReactiveObject
        {
        }

        public interface Test : ReactiveUI.IReactiveObject
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.ReactiveObjectInterfaceShouldHaveViewModelSuffix,
                Message = NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 26)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzer();
        }
    }
}
