﻿using System;
using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class TryParseShouldBeUsedInLogicalNotIfStatementAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        internal const string Title = "TryParse should be used inside an If Statement with a unary operation";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Usage;

        private const string Description =
            "TryParse should be used inside an If Statement with a unary operation. This allows mitigating errors with parsing.";

        /// <summary>
        /// Creates an instance of ConstructorShouldNotInvokeExternalMethodsAnalyzer
        /// </summary>
        public TryParseShouldBeUsedInLogicalNotIfStatementAnalyzer()
        {
            this._rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.TryParseShouldBeUsedInLogicalNotIfStatement,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Description);
        }

        /// <inhertitdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this._rule);

        /// <inhertitdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated())
            {
                return;
            }

            var node = context.Node;

            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (!(invocationExpression.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax))
            {
                return;
            }

            if (!memberAccessExpressionSyntax.Name.Identifier.Text.Equals("TryParse", StringComparison.Ordinal))
            {
                return;
            }

            if (IsInIfStatementWithLogicalNotOperation(invocationExpression))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(this._rule, node.GetLocation()));
        }

        private bool IsInIfStatementWithLogicalNotOperation(SyntaxNode syntaxNode)
        {
            var prefixUnaryExpressionSyntax = GetParentNodeOfType<PrefixUnaryExpressionSyntax>(syntaxNode);

            if (prefixUnaryExpressionSyntax == null)
            {
                return false;
            }

            var ifStatementSyntax = GetParentNodeOfType<PrefixUnaryExpressionSyntax>(prefixUnaryExpressionSyntax);

            return ifStatementSyntax != null;
        }

        private static T GetParentNodeOfType<T>(SyntaxNode syntaxNode)
            where T : SyntaxNode
        {
            var currentNode = syntaxNode.Parent;
            while (currentNode != null)
            {

                if (currentNode is T parent)
                {
                    return parent;
                }

                currentNode = currentNode.Parent;
            }

            return null;
        }
    }
}