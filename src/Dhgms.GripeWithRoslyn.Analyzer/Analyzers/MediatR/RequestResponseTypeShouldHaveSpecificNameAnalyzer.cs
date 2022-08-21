// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.MediatR
{
    /// <summary>
    /// Analyzer to check that the response type argument Mediatr IRequest ends with "CommandResponse" or "QueryResponse".
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class RequestResponseTypeShouldHaveSpecificNameAnalyzer : AbstractTypeArgumentForClassShouldEndWithSpecificSuffixAnalyzer
    {
        internal const string Title = "Response Classes used in MediatR.IRequest should end with either the suffix \"CommandResponse\" or \"QueryResponse\".";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Naming;

        private const string Description =
            "MediatR Responses should follow a consistent naming convention indicating if it is from a Commands or Queries.";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestResponseTypeShouldHaveSpecificNameAnalyzer"/> class.
        /// </summary>
        public RequestResponseTypeShouldHaveSpecificNameAnalyzer()
            : base(
                DiagnosticIdsHelper.MediatRResponseShouldHaveCommandResponseOrQueryResponseSuffix,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc/>
        protected override string[] ClassNameSuffixes => new[]
        {
            "CommandResponse",
            "QueryResponse"
        };

        /// <inheritdoc/>
        protected override string BaseClassFullName => "global::MediatR.IRequest";
    }
}
