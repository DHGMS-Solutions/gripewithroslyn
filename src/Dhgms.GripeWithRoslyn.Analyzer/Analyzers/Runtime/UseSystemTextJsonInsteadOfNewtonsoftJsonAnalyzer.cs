// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime
{
    /// <summary>
    /// Analyzer to suggest the use of <see cref="N:System.Text.Json"/> instead of <see cref="N:Newtonsoft.Json"/>.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer : BaseInvocationUsingNamespaceAnalyzer
    {
        internal const string Title = "Consider use of System.Text.Json instead of Newtonsoft.Json (JSON.NET).";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "System.Text.Json brings improvements from JSON.NET.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer"/> class.
        /// </summary>
        public UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer()
            : base(
            DiagnosticIdsHelper.UseSystemTextJsonInsteadOfNewtonsoftJson,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string Namespace => "global::Newtonsoft.Json";
    }
}
