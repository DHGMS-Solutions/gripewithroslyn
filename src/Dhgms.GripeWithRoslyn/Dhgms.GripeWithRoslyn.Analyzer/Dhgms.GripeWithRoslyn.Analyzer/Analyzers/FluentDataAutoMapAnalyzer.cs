namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using CodeCracker;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Roslyn Analyzer to check for uses of FluentData's AutoMap method
    /// </summary>
    /// <remarks>
    /// Based upon : https://raw.githubusercontent.com/code-cracker/code-cracker/master/src/CSharp/CodeCracker/Performance/UseStaticRegexIsMatchAnalyzer.cs
    /// </remarks>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class FluentDataAutoMapAnalyzer : BaseInvocationExpressionAnalyzer
    {
        public const string DiagnosticId = "DhgmsGripeWithRoslynAnalyzer";

        private const string Title = "FluentData AutoMap should not be used.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "AutoMap produces potential technical debt where if you are preparing the database schema for new content the old POCO objects won't map due to not having the corresponding property. This risks taking down your platform\\service. Please use a mapper.";

        protected override string MethodName => "AutoMap";

        protected override string[] ContainingTypes => new[]
                {
                    "FluentData.IInsertBuilder<T>",
                    "FluentData.IStoredProcedureBuilderDynamic",
                    "FluentData.IStoredProcedureBuilder<T>",
                    "FluentData.IUpdateBuilderDynamic",
                    "FluentData.IUpdateBuilder<T>",
                };

        public FluentDataAutoMapAnalyzer()
            : base("GR0001",
                  Title,
                  MessageFormat,
                  Category,
                  Description,
                  DiagnosticSeverity.Warning)
        {
        }
    }
}
