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
    /// Analyzer to ensure a class inheriting from MediatR.IRequest has the suffix ViewModel.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class NameOfRequestShouldEndWithCommandOrQueryAnalyzer : BaseClassInheritingTypeShouldEndWithSpecificSuffix
    {
        internal const string Title = "Classes based on MediatR.IRequest should end with either the suffix \"Command\" or \"Query\".";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "MediatR Requests should follow a consistent naming convention indicating if it is for Commands or Queries.";

        /// <summary>
        /// Initializes a new instance of the <see cref="NameOfRequestShouldEndWithCommandOrQueryAnalyzer"/> class.
        /// </summary>
        public NameOfRequestShouldEndWithCommandOrQueryAnalyzer()
            : base(
                DiagnosticIdsHelper.MediatRRequestShouldHaveCommandOrQuerySuffix,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string[] NameSuffixes => new[]
        {
            "Command",
            "Query"
        };

        /// <inheritdoc />
        protected override string BaseClassFullName => "global::MediatR.IRequest";
    }
}
