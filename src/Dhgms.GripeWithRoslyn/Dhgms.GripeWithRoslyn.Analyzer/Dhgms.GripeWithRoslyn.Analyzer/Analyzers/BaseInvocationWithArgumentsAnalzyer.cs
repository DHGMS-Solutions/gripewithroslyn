namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using System;
    using System.Linq;
    using System.Collections.Immutable;

    using CodeCracker;

    using JetBrains.Annotations;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    public abstract class BaseInvocationWithArgumentsAnalzyer : DiagnosticAnalyzer
    {
        protected BaseInvocationWithArgumentsAnalzyer(
            [NotNull] string diagnosticId,
            [NotNull] string title,
            [NotNull] string message,
            [NotNull] string category,
            [NotNull] string description,
            DiagnosticSeverity diagnosticSeverity)
        {
            this.Rule = new DiagnosticDescriptor(diagnosticId, title, message, category, diagnosticSeverity, isEnabledByDefault: true, description: description);
        }

        protected DiagnosticDescriptor Rule { get; private set; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this.Rule);

        [NotNull]
        protected abstract string MethodName { get; }

        [NotNull]
        protected abstract string[] ContainingTypes { get; }

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <remarks>
        /// https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        /// </remarks>
        /// <param name="context">
        /// Roslyn context.
        /// </param>
        public sealed override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated())
            {
                return;
            }

            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberExpression == null || !memberExpression.Name.ToString().Equals(MethodName, StringComparison.Ordinal))
            {
                return;
            }

            var methodSymbol = context.SemanticModel.GetSymbolInfo(memberExpression).Symbol;
            if (methodSymbol == null
                || ContainingTypes.All(x => !methodSymbol.ContainingType.OriginalDefinition.ToString().Equals(x, StringComparison.Ordinal)))
            {
                return;
            }

            OnValidateArguments(context, invocationExpression.ArgumentList);
        }

        protected abstract void OnValidateArguments(SyntaxNodeAnalysisContext context, ArgumentListSyntax argumentList);
    }
}
