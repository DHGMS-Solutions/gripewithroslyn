// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions
{
    /// <summary>
    /// These analyzers are for where you don't care about the actual method but more the namespace it is inside.
    /// Useful when you associate an old namespace with legacy code you don't want being used.
    /// For example .NET remoting.
    /// </summary>
    public abstract class BaseInvocationUsingNamespaceAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseInvocationUsingNamespaceAnalyzer"/> class.
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id.</param>
        /// <param name="title">The title of the analyzer.</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer.</param>
        /// <param name="diagnosticSeverity">The severity associated with breaches of the analyzer.</param>
        protected BaseInvocationUsingNamespaceAnalyzer(
            [NotNull] string diagnosticId,
            [NotNull] string title,
            [NotNull] string message,
            [NotNull] string category,
            [NotNull] string description,
            DiagnosticSeverity diagnosticSeverity)
        {
            _rule = new DiagnosticDescriptor(
                diagnosticId,
                title,
                message,
                category,
                diagnosticSeverity,
                isEnabledByDefault: true,
                description: description);
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(_rule);

        /// <summary>
        /// Gets the namespace to check for.
        /// </summary>
        [NotNull]
        protected abstract string Namespace { get; }

        /// <inheritdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberExpression == null)
            {
                return;
            }

            var methodSymbol = context.SemanticModel.GetSymbolInfo(memberExpression).Symbol;

            var containingNamespace = methodSymbol?.OriginalDefinition.ContainingNamespace;
            if (containingNamespace == null
                || !containingNamespace.GetFullName().StartsWith(Namespace, StringComparison.Ordinal))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, invocationExpression.GetLocation()));
        }

        private void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (ObjectCreationExpressionSyntax)context.Node;

            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol;

            var containingNamespace = methodSymbol?.OriginalDefinition.ContainingNamespace;
            if (containingNamespace == null
                || !containingNamespace.GetFullName().StartsWith(Namespace, StringComparison.Ordinal))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, invocationExpression.GetLocation()));
        }
    }
}
