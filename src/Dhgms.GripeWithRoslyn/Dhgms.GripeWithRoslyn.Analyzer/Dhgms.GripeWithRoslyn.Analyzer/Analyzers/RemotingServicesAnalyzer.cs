namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using CodeCracker;

    using Microsoft.CodeAnalysis;

    public sealed class RemotingServicesAnalyzer : BaseInvocationUsingNamespaceAnalyzer
    {
        public const string DiagnosticId = "DhgmsGripeWithRoslynAnalyzer";

        private const string Title = ".NET remoting is legacy technology and should not be used. You should be using WCF\\WebAPI.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            ".NET remoting is legacy technology and should not be used. You should be using WCF\\WebAPI.";

        public RemotingServicesAnalyzer()
            : base("GR0004",
                  Title,
                  MessageFormat,
                  Category,
                  Description,
                  DiagnosticSeverity.Warning)
        {
        }

        protected override string Namespace => "System.Runtime.Remoting";
    }
}
