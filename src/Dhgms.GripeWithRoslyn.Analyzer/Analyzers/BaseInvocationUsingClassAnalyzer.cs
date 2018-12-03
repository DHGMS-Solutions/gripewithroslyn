using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCracker;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    public abstract class BaseInvocationUsingClassAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Creates an instance of BaseInvocationUsingClassAnalyzer
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id</param>
        /// <param name="title">The title of the analyzer</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer</param>
        /// <param name="diagnosticSeverity">The severity assocatiated with breaches of the analyzer</param>
        protected BaseInvocationUsingClassAnalyzer(
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
            if (memberExpression == null)
            {
                return;
            }

            var methodSymbol = ModelExtensions.GetSymbolInfo(context.SemanticModel, memberExpression).Symbol;

            var containingAssembly = methodSymbol?.OriginalDefinition.ContainingType;
            if (containingAssembly == null
                || !containingAssembly.GetFullName().Equals(this.ClassName, StringComparison.Ordinal))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(this._rule, invocationExpression.GetLocation()));
        }

        protected abstract string ClassName { get; }
    }
}
