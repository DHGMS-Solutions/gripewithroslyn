// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime
{
    /// <summary>
    /// Analyzer to detect the use of <see cref="System.Net.ServicePointManager"/>.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class DoNotUseSystemNetServicePointManagerAnalyzer : BaseInvocationUsingClassAnalyzer
    {
        internal const string Title = "Do not use System.Net.ServicePointManager.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "System.Net.ServicePointManager suggests code that may not be flexible, such as making global changes to transport layer security. The recommendation is to use System.Net.HttpClient";

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseSystemNetServicePointManagerAnalyzer"/> class.
        /// </summary>
        public DoNotUseSystemNetServicePointManagerAnalyzer()
            : base(
            DiagnosticIdsHelper.DoNotUseSystemNetServicePointManager,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc/>
        protected override string ClassName => "global::System.Net.ServicePointManager";
    }
}