﻿using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class DoNotUseGdiPlusAnalyzer : BaseInvocationUsingNamespaceAnalyzer
    {
        internal const string Title = "Do not use GDI+.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "GDI+ usage needs to be considered as it is not suitable for web development etc.";

        public DoNotUseGdiPlusAnalyzer() : base(
            DiagnosticIdsHelper.DoNotUseGdiPlus,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        protected override string Namespace => "global::System.Drawing";
    }
}
