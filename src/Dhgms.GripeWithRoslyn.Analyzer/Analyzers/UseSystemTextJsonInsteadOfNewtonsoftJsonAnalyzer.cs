using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer : BaseInvocationUsingNamespaceAnalyzer
    {
        internal const string Title = "Consider use of System.Text.Json instead of Newtonsoft.Json (JSON.NET).";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "System.Text.Json brings improvements from JSON.NET.";

        /// <summary>
        /// Creates an instance of UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer
        /// </summary>
        public UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer() : base(
            DiagnosticIdsHelper.UseSystemTextJsonInsteadOfNewtonsoftJson,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        /// <inhertitdoc />
        protected override string Namespace => "global::Newtonsoft.Json";
    }
}
