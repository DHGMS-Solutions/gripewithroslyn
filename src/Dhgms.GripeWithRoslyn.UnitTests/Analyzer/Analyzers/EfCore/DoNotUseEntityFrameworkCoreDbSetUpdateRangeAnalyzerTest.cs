// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.EfCore;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.EfCore
{
    /// <summary>
    /// Unit test for <see cref="DoNotUseEntityFrameworkCoreDbSetUpdateAnalyzer"/>.
    /// </summary>
    public sealed class DoNotUseEntityFrameworkCoreDbSetUpdateRangeAnalyzerTest : AbstractAnalyzerTest<DoNotUseEntityFrameworkCoreDbSetUpdateRangeAnalyzer>
    {
        /// <inheritdoc/>
        protected override string GetExpectedDiagnosticId()
        {
            return DiagnosticIdsHelper.DoNotUseEntityFrameworkCoreDbSetUpdateRange;
        }

        /// <inheritdoc/>
        protected override ExpectedDiagnosticModel[] GetExpectedDiagnosticLines()
        {
            return
            [
                new ExpectedDiagnosticModel(
                    "EfCore\\DbSetUpdateProof.cs",
                    DiagnosticSeverity.Error,
                    53,
                    12)
            ];
        }
    }
}