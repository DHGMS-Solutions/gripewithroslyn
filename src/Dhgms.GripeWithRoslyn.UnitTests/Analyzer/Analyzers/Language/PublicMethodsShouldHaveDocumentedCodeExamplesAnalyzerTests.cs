// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language;
using Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.EfCore;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Unit test for <see cref="PublicMethodsShouldHaveDocumentedCodeExamplesAnalyzer"/>.
    /// </summary>
    public sealed class PublicMethodsShouldHaveDocumentedCodeExamplesAnalyzerTests : AbstractAnalyzerTest<PublicMethodsShouldHaveDocumentedCodeExamplesAnalyzer>
    {
        /// <inheritdoc/>
        protected override string GetExpectedDiagnosticId()
        {
            return DiagnosticIdsHelper.PublicMethodsShouldHaveDocumentedCodeExamples;
        }

        /// <inheritdoc/>
        protected override ExpectedDiagnosticModel[] GetExpectedDiagnosticLines()
        {
            return
            [
                new ExpectedDiagnosticModel(
                    "EfCore\\DbSetUpdateProof.cs",
                    DiagnosticSeverity.Error,
                    27,
                    27)
            ];
        }
    }
}
