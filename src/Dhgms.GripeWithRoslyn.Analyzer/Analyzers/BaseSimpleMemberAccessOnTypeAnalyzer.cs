using System;
using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    public abstract class BaseSimpleMemberAccessOnTypeAnalyzer : DiagnosticAnalyzer
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
        protected BaseSimpleMemberAccessOnTypeAnalyzer(
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

        /// <inhertitdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(this._rule);

        /// <inhertitdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated())
            {
                return;
            }

            var memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)context.Node;

            if (memberAccessExpressionSyntax == null
                || memberAccessExpressionSyntax.Expression == null)
            {
                return;
            }

            if (!memberAccessExpressionSyntax.Name.ToString().Equals(MemberName, StringComparison.Ordinal))
            {
                return;
            }

            var typeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, memberAccessExpressionSyntax.Expression);

            if (typeInfo.Type == null)
            {
                return;
            }

            var typeFullName = typeInfo.Type.GetFullName();

            if (!typeFullName.Equals(this.ClassName, StringComparison.Ordinal))
            {
                return;
            }


            context.ReportDiagnostic(Diagnostic.Create(this._rule, memberAccessExpressionSyntax.GetLocation()));
        }

        protected abstract string ClassName { get; }

        protected abstract string MemberName { get; }
    }
}
