using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Roslyn Analyzer to check for uses of FluentData's AutoMap method
    /// </summary>
    /// <remarks>
    /// Based upon : https://raw.githubusercontent.com/Wintellect/Wintellect.Analyzers/master/Source/Wintellect.Analyzers/Wintellect.Analyzers/Usage/CallAssertMethodsWithMessageParameterAnalyzer.cs
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class FluentDataQueryManyWithNullArgument : BaseInvocationWithArgumentsAnalzyer
    {
        private const string Title = "FluentData QueryMany should not be used with a null value for the mapper.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "QueryMany without a mapper produces potential technical debt where if you are preparing the database schema for new content the old POCO objects won't map due to not having the corresponding property. This risks taking down your platform\\service. Please use a mapper.";

        /// <summary>
        /// Creates an instance of FluentDataQueryManyWithNullArgument
        /// </summary>
        public FluentDataQueryManyWithNullArgument()
            : base(
                  DiagnosticIdsHelper.FluentDataQueryManyWithNullArgumentAnalyzer,
                  Title,
                  MessageFormat,
                  Category,
                  Description,
                  DiagnosticSeverity.Warning)
        {
        }

        /// <summary>
        /// The name of the method to check for.
        /// </summary>
        protected override string MethodName => "QueryMany";

        /// <summary>
        /// The classes the method may belong to.
        /// </summary>
        protected override string[] ContainingTypes => new[]
                {
                    "FluentData.ISelectBuilder<TEntity>",
                    "FluentData.IStoredProcedureBuilder<T>",
                    "FluentData.IStoredProcedureBuilderDynamic",
                    "FluentData.IDbCommand",
                    "FluentData.IQuery"
                };

        /// <summary>
        /// Event for validating the arguments passed
        /// </summary>
        /// <param name="context">The context for the Roslyn syntax analysis</param>
        /// <param name="argumentList">Syntax representation of the argument list.</param>
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
