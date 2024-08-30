// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Analyzer for warning on usages of method groups.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseMethodGroupsAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Do not use method groups. For readability and consistency consider using lambda with manual caching if performance is an issue.";
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseMethodGroupsAnalyzer"/> class.
        /// </summary>
        public DoNotUseMethodGroupsAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.DoNotUseMethodGroups,
                Title,
                Title,
                SupportedCategories.Maintainability,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Title);
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterSyntaxNodeAction(AnalyzeIdentifierName, SyntaxKind.IdentifierName);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private static bool IsMethodGroupInvocation(SyntaxNodeAnalysisContext context, IdentifierNameSyntax identifierName)
        {
            // Check if the identifier refers to a method symbol and is used as a delegate
            var symbolInfo = context.SemanticModel.GetSymbolInfo(identifierName);
            if (symbolInfo.Symbol is IMethodSymbol _)
            {
                // Ensure it's not followed by parentheses (indicating an invocation)
                var parent = identifierName.Parent;
                if (parent is ArgumentSyntax)
                {
                    return true;
                }
            }

            return false;
        }

        private void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context)
        {
            var identifierNode = (IdentifierNameSyntax)context.Node;

            if (!IsMethodGroupInvocation(context, identifierNode))
            {
                return;
            }

            var diagnostic = Diagnostic.Create(_rule, identifierNode.GetLocation(), identifierNode.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            foreach (var argument in invocationExpression.ArgumentList.Arguments)
            {
                if (argument.Expression is not IdentifierNameSyntax identifierName)
                {
                    continue;
                }

                if (!IsMethodGroupInvocation(context, identifierName))
                {
                    continue;
                }

                var diagnostic = Diagnostic.Create(_rule, identifierName.GetLocation(), identifierName.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
