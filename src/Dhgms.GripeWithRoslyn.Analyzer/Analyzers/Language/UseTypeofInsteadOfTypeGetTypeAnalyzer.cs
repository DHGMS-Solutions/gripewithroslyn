// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Analyzer to suggest using typeof() instead of <see cref="M:System.Type.GetType"/>.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class UseTypeofInsteadOfTypeGetTypeAnalyzer : BaseInvocationExpressionAnalyzer
    {
        private const string Title = "Consider usage of typeof(x) instead of System.Type.GetType.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "System.Type.GetType may be a misuse of establishling type information. typeof(x) allows for compile time verification of the type.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UseTypeofInsteadOfTypeGetTypeAnalyzer"/> class.
        /// </summary>
        public UseTypeofInsteadOfTypeGetTypeAnalyzer()
            : base(
            DiagnosticIdsHelper.UseTypeofInsteadOfSystemTypeGetType,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string MethodName => "GetType";

        /// <inheritdoc />
        protected override string[] ContainingTypes => new[] { "global::System.Type" };
    }
}
