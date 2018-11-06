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

    /// <summary>
    /// Base class for a Roslyn Analayzer for Method Invocation where you need to validate the arguments passed.
    /// </summary>
    public abstract class BaseInvocationWithArgumentsAnalzyer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Creates an instance of BaseInvocationWithArgumentsAnalzyer
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id</param>
        /// <param name="title">The title of the analyzer</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer</param>
        /// <param name="diagnosticSeverity">The severity assocatiated with breaches of the analyzer</param>
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


        /// <summary>
        /// Description of the Diagnostic Rule. Used when passing details to Roslyn.
        /// </summary>
        protected DiagnosticDescriptor Rule { get; }

        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this.Rule);

        /// <summary>
        /// The name of the method to check for.
        /// </summary>
        [NotNull]
        protected abstract string MethodName { get; }

        /// <summary>
        /// The classes the method may belong to.
        /// </summary>
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

        /// <summary>
        /// Event for validating the arguments passed
        /// </summary>
        /// <param name="context">The context for the Roslyn syntax analysis</param>
        /// <param name="argumentList">Syntax representation of the argument list.</param>
        protected abstract void OnValidateArguments(SyntaxNodeAnalysisContext context, ArgumentListSyntax argumentList);
    }
}
