using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using System.Collections.Immutable;

    using CodeCracker;

    using JetBrains.Annotations;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    public abstract class BaseInvocationUsingDllAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        protected BaseInvocationUsingDllAnalyzer(
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

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(this._rule);

        [NotNull]
        protected abstract string AssemblyName { get; }

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

            if (context.IsGenerated()) return;
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberExpression == null)
            {
                return;
            }

            var methodSymbol = context.SemanticModel.GetSymbolInfo(memberExpression).Symbol;

            var containingAssembly = methodSymbol?.OriginalDefinition.ContainingAssembly;
            if (containingAssembly == null
                || !containingAssembly.Name.Equals(this.AssemblyName, StringComparison.Ordinal))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(this._rule, invocationExpression.GetLocation()));
        }
    }
}