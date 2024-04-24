// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.AppCenter
{
    /// <summary>
    /// Analyzer to ensure structure map is not used.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class AppCenterShouldNotBeUsedAnalyzer : BaseInvocationUsingNamespaceAnalyzer
    {
        private const string Title = "Microsoft AppCenter is end of life so should not be used.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "The Microsoft AppCenter product has been retired. This codebase should not be used.";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCenterShouldNotBeUsedAnalyzer"/> class.
        /// </summary>
        public AppCenterShouldNotBeUsedAnalyzer()
            : base(
                DiagnosticIdsHelper.MicrosoftAppCenterShouldNotBeUsed,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string Namespace => "global::Microsoft.AppCenter";
    }
}
