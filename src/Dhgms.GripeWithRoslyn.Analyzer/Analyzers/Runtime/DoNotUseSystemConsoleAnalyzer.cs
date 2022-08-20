// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime
{
    /// <summary>
    /// Analyzer to enure members of System.Console are not used.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class DoNotUseSystemConsoleAnalyzer : BaseInvocationUsingClassAnalyzer
    {
        internal const string Title = "Do not use System.Console.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "System.Console suggests code that may not be flexible, or carrying out unintended work such as not using a proper logging implementation.";

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseSystemConsoleAnalyzer"/> class.
        /// </summary>
        public DoNotUseSystemConsoleAnalyzer()
            : base(
            DiagnosticIdsHelper.DoNotUseSystemConsole,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string ClassName => "global::System.Console";
    }
}
