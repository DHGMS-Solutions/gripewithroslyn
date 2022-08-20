// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Analyzer to check if TryParse is used in a logical if statement.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class BooleanTryMethodShouldBeUsedInLogicalNotIfStatementAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "TryParse should be used inside an If Statement with a unary operation";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Usage;

        private const string Description =
            "TryParse should be used inside an If Statement with a unary operation. This allows mitigating errors with parsing.";

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanTryMethodShouldBeUsedInLogicalNotIfStatementAnalyzer"/> class.
        /// </summary>
        public BooleanTryMethodShouldBeUsedInLogicalNotIfStatementAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.TryParseShouldBeUsedInLogicalNotIfStatement,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Description);
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
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

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (!(invocationExpression.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax))
            {
                return;
            }

            if (!memberAccessExpressionSyntax.Name.Identifier.Text.StartsWith("Try", StringComparison.Ordinal))
            {
                return;
            }

            var invocationExpressionTypeInfo = context.SemanticModel.GetTypeInfo(invocationExpression);

            var type = invocationExpressionTypeInfo.Type;

            if (type?.SpecialType != SpecialType.System_Boolean)
            {
                return;
            }

            if (IsInIfStatementWithLogicalNotOperation(invocationExpression))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, node.GetLocation()));
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
    }
}