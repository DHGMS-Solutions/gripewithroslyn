// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.StructureMap
{
    /// <summary>
    /// Analyzer to ensure structure map is not used.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class StructureMapShouldNotBeUsedAnalyzer : BaseInvocationUsingNamespaceAnalyzer
    {
        private const string Title = "StructureMap is end of life so should not be used.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "The StructureMap project has been retired. This means there are no gurantees of fixes. This codebase should not be used.";

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapShouldNotBeUsedAnalyzer"/> class.
        /// </summary>
        public StructureMapShouldNotBeUsedAnalyzer()
            : base(
            DiagnosticIdsHelper.StructureMapShouldNotBeUsed,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string Namespace => "global::StructureMap";
    }
}
