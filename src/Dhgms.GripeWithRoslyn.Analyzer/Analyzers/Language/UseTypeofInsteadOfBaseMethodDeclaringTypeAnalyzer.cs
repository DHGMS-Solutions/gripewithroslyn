// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Analyzer to suggest the use of typeof() instead of <see cref="System.Reflection.MethodBase.GetCurrentMethod()"/> and <see cref="System.Reflection.MemberInfo.DeclaringType"/> .
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class UseTypeofInsteadOfBaseMethodDeclaringTypeAnalyzer : BaseSimpleMemberAccessOnTypeAnalyzer
    {
        private const string Title = "Consider usage of typeof(x) instead of MethodBase.GetCurrentMethod().DeclaringType.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "MethodBase.GetCurrentMethod().DeclaringType may be a misuse of establishling type information. typeof(x) allows for compile time verification of the type and avoids reflection.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UseTypeofInsteadOfBaseMethodDeclaringTypeAnalyzer"/> class.
        /// </summary>
        public UseTypeofInsteadOfBaseMethodDeclaringTypeAnalyzer()
            : base(
            DiagnosticIdsHelper.UseTypeofInsteadOfMethodBaseGetCurrentMethodDeclaringType,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string ClassName => "global::System.Reflection.MethodBase";

        /// <inheritdoc />
        protected override string MemberName => "DeclaringType";
    }
}
