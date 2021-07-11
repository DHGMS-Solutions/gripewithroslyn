using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    public sealed class DoNotUseSystemNetServicePointManagerAnalyzer : BaseInvocationUsingClassAnalyzer
    {
        internal const string Title = "Do not use System.Net.ServicePointManager.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "System.Net.ServicePointManager suggests code that may not be flexible, such as making global changes to transport layer security. The recommendation is to use System.Net.HttpClient";

        public DoNotUseSystemNetServicePointManagerAnalyzer() : base(
            DiagnosticIdsHelper.DoNotUseSystemNetServicePointManager,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        protected override string ClassName => "global::System.Net.ServicePointManager";
    }
}
