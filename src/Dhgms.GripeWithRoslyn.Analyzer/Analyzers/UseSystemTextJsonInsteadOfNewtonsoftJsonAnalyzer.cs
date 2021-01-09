using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    public sealed class UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer : BaseInvocationUsingNamespaceAnalyzer
    {
        internal const string Title = "Consider use of System.Text.Json instead of Newtonsoft.Json (JSON.NET).";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "System.Text.Json brings improvements from JSON.NET.";

        public UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer() : base(
            DiagnosticIdsHelper.UseSystemTextJsonInsteadOfNewtonsoftJson,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        protected override string Namespace => "global::Newtonsoft.Json";
    }
}
