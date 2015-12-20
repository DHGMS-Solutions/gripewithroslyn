namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using System;
    using System.Collections.Immutable;

    using CodeCracker;

    using JetBrains.Annotations;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Roslyn Analyzer to check for uses of FluentData's AutoMap method
    /// </summary>
    /// <remarks>
    /// Based upon : https://raw.githubusercontent.com/Wintellect/Wintellect.Analyzers/master/Source/Wintellect.Analyzers/Wintellect.Analyzers/Usage/CallAssertMethodsWithMessageParameterAnalyzer.cs
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class FluentDataQueryManyWithNullArgument : BaseInvocationWithArgumentsAnalzyer
    {
        public const string DiagnosticId = "DhgmsGripeWithRoslynAnalyzer";

        private const string Title = "FluentData AutoMap should not be used.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "QueryMany without a mapper produces potential technical debt where if you are preparing the database schema for new content the old POCO objects won't map due to not having the corresponding property. This risks taking down your platform\\service. Please use a mapper.";

        public FluentDataQueryManyWithNullArgument()
            : base(
                  "GR0002",
                  Title,
                  MessageFormat,
                  Category,
                  Description,
                  DiagnosticSeverity.Warning)
        {
        }

        protected override string MethodName => "QueryMany";

        protected override string[] ContainingTypes => new[]
                {
                    "FluentData.IInsertBuilder<TEntity>",
                    "FluentData.IStoredProcedureBuilder<TEntity>",
                    "FluentData.IUpdateBuilder<TEntity>",
                    "FluentData.ISelectBuilder<TEntity>"
                };

        protected override void OnValidateArguments(SyntaxNodeAnalysisContext context, ArgumentListSyntax argumentList)
        {
            if (argumentList?.Arguments.Count != 1)
            {
                return;
            }

            // todo: try and remove the tostring call
            var arg = argumentList.Arguments[0];
            if (arg.Expression.ToString().Equals("null", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(Rule, arg.Expression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }

        }
    }
}
