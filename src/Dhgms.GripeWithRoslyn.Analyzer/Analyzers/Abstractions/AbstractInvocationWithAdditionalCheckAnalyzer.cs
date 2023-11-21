// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions
{
    /// <summary>
    /// Abstraction of a method invocation check based on the method name and then some extended logic in a subclass.
    /// </summary>
    public abstract class AbstractInvocationWithAdditionalCheckAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractInvocationWithAdditionalCheckAnalyzer"/> class.
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id.</param>
        /// <param name="title">The title of the analyzer.</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer.</param>
        /// <param name="diagnosticSeverity">The severity associated with breaches of the analyzer.</param>
        protected AbstractInvocationWithAdditionalCheckAnalyzer(
            string diagnosticId,
            string title,
            string message,
            string category,
            string description,
            DiagnosticSeverity diagnosticSeverity)
        {
            _rule = new DiagnosticDescriptor(diagnosticId, title, message, category, diagnosticSeverity, isEnabledByDefault: true, description: description);
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <summary>
        /// Gets the name of the method to check for.
        /// </summary>
        protected abstract string MethodName { get; }

        /// <summary>
        /// Gets the types the method may belong to.
        /// </summary>
        protected abstract string[] ContainingTypes { get; }

        /// <inheritdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        /// <summary>
        /// Check if a diagnostic report should be created based on custom logic in the subclass.
        /// </summary>
        /// <param name="semanticModel">Roslyn semantic model for the code being analyzed.</param>
        /// <param name="memberExpression">Member access expression to do extended checks on.</param>
        /// <returns>Whether a diagnostic report should be created.</returns>
        protected abstract bool GetIfShouldReport(
            SemanticModel semanticModel,
            MemberAccessExpressionSyntax memberExpression);

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberExpression == null || !memberExpression.Name.ToString().Equals(MethodName, StringComparison.Ordinal))
            {
                return;
            }

            if (!GetIfShouldReport(context.SemanticModel, memberExpression))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, invocationExpression.GetLocation()));
        }
    }
}