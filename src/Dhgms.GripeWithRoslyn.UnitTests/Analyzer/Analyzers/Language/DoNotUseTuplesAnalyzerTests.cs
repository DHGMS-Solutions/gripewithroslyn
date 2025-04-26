// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.EfCore;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language;
using Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.EfCore;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Unit Tests for <see cref="DoNotUseTuplesAnalyzer"/>.
    /// </summary>
    public sealed class DoNotUseTuplesAnalyzerTests : AbstractAnalyzerTest<DoNotUseTuplesAnalyzer>
    {
        /// <inheritdoc/>
        protected override string GetExpectedDiagnosticId()
        {
            return DiagnosticIdsHelper.DoNotUseTuples;
        }

        /// <inheritdoc/>
        protected override ExpectedDiagnosticModel[] GetExpectedDiagnosticLines()
        {
            const string TupleProofFilePath = "Language\\TupleProof.cs";

            return
            [
                new ExpectedDiagnosticModel(
                    TupleProofFilePath,
                    DiagnosticSeverity.Error,
                    23,
                    22),

                new ExpectedDiagnosticModel(
                    TupleProofFilePath,
                    DiagnosticSeverity.Error,
                    23,
                    23),

                new ExpectedDiagnosticModel(
                    TupleProofFilePath,
                    DiagnosticSeverity.Error,
                    23,
                    35),

                new ExpectedDiagnosticModel(
                    TupleProofFilePath,
                    DiagnosticSeverity.Error,
                    25,
                    19),
            ];
        }
    }
}
