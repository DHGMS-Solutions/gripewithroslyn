// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime
{
    /// <summary>
    /// Analyzer to suggest using <see cref="T:System.Text.UnicodeEncoding"/> instead of <see cref="T:System.Text.ASCIIEncoding"/>.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class UseEncodingUnicodeInsteadOfASCIIAnalyzer : BaseInvocationUsingClassAnalyzer
    {
        private const string Title = "Consider usage of Unicode Encoding instead of ASCII.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Reliability;

        private const string Description =
            "ASCII encoding may cause loss of information if Unicode characters are in use. Consider using System.Text.UnicodeEncoding";

        /// <summary>
        /// Initializes a new instance of the <see cref="UseEncodingUnicodeInsteadOfASCIIAnalyzer"/> class.
        /// </summary>
        public UseEncodingUnicodeInsteadOfASCIIAnalyzer()
            : base(
                DiagnosticIdsHelper.UseEncodingUnicodeInsteadOfAscii,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string ClassName => "global::System.Text.ASCIIEncoding";
    }
}
