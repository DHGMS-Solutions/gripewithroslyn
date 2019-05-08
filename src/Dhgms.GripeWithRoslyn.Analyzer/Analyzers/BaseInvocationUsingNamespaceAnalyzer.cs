using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using JetBrains.Annotations;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    /// <summary>
    /// These analyzers are for where you don't care about the actual method but more the namespace it is inside.
    /// Useful when you associate an old namespace with legacy code you don't want being used.
    /// For example .NET remoting
    /// </summary>
    public abstract class BaseInvocationUsingNamespaceAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Creates an instance of BaseInvocationUsingNamespaceAnalyzer
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id</param>
        /// <param name="title">The title of the analyzer</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer</param>
        /// <param name="diagnosticSeverity">The severity assocatiated with breaches of the analyzer</param>
        protected BaseInvocationUsingNamespaceAnalyzer(
            [NotNull] string diagnosticId,
            [NotNull] string title,
            [NotNull] string message,
            [NotNull] string category,
            [NotNull] string description,
            DiagnosticSeverity diagnosticSeverity)
        {
            this._rule = new DiagnosticDescriptor(
                diagnosticId,
                title,
                message,
                category,
                diagnosticSeverity,
                isEnabledByDefault: true,
                description: description);
        }

        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(this._rule);

        /// <summary>
        /// The namespace to check for.
        /// </summary>
        [NotNull]
        protected abstract string Namespace { get; }

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
            context.RegisterSyntaxNodeAction(this.AnalyzeObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated())
            {
                return;
            }

            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberExpression == null)
            {
                return;
            }

            var methodSymbol = context.SemanticModel.GetSymbolInfo(memberExpression).Symbol;

            var containingNamespace = methodSymbol?.OriginalDefinition.ContainingNamespace;
            if (containingNamespace == null
                || !containingNamespace.GetFullName().StartsWith(this.Namespace, StringComparison.Ordinal))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(this._rule, invocationExpression.GetLocation()));
        }

        private void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated())
            {
                return;
            }

            var invocationExpression = (ObjectCreationExpressionSyntax)context.Node;

            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol;

            var containingNamespace = methodSymbol?.OriginalDefinition.ContainingNamespace;
            if (containingNamespace == null
                || !containingNamespace.GetFullName().StartsWith(this.Namespace, StringComparison.Ordinal))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(this._rule, invocationExpression.GetLocation()));
        }
    }
}
