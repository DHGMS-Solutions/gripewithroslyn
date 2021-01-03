using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    public sealed class StructureMapShouldNotBeUsedAnalyzer : BaseInvocationUsingNamespaceAnalyzer
    {
        private const string Title = "StructureMap is end of life so should not be used.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "The StructureMap project has been retired. This means there are no gurantees of fixes. This codebase should not be used.";

        public StructureMapShouldNotBeUsedAnalyzer() : base(
            DiagnosticIdsHelper.StructureMapShouldNotBeUsed,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        protected override string Namespace => "global::StructureMap";
    }
}
