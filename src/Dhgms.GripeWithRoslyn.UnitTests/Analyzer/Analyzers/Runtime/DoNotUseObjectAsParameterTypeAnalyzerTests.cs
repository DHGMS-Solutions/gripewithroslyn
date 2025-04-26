// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime;
using Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.EfCore;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.Runtime
{
    /// <summary>
    /// Unit Tests for <see cref="DoNotUseObjectAsParameterTypeAnalyzer"/>.
    /// </summary>
    public sealed class DoNotUseObjectAsParameterTypeAnalyzerTests : AbstractAnalyzerTest<DoNotUseObjectAsParameterTypeAnalyzer>
    {
        /// <inheritdoc/>
        protected override string GetExpectedDiagnosticId()
        {
            return DiagnosticIdsHelper.DoNotUseObjectAsParameterType;
        }

        /// <inheritdoc />
        protected override ExpectedDiagnosticModel[] GetExpectedDiagnosticLines()
        {
            return
            [
                new ExpectedDiagnosticModel(
                    "Runtime\\DoNotUseObjectAsParameterTypeProof.cs",
                    DiagnosticSeverity.Error,
                    66,
                    12),
                new ExpectedDiagnosticModel(
                    "Runtime\\DoNotUseObjectAsParameterTypeProof.cs",
                    DiagnosticSeverity.Error,
                    86,
                    31),
                new ExpectedDiagnosticModel(
                    "Runtime\\DoNotUseObjectAsParameterTypeProof.cs",
                    DiagnosticSeverity.Error,
                    104,
                    32),
            ];
        }
    }
}
