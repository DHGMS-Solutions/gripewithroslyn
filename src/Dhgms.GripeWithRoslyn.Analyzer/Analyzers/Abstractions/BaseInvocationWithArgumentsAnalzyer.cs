// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions
{
    /// <summary>
    /// Base class for a Roslyn Analayzer for Method Invocation where you need to validate the arguments passed.
    /// </summary>
    public abstract class BaseInvocationWithArgumentsAnalzyer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseInvocationWithArgumentsAnalzyer"/> class.
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id.</param>
        /// <param name="title">The title of the analyzer.</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer.</param>
        /// <param name="diagnosticSeverity">The severity associated with breaches of the analyzer.</param>
        protected BaseInvocationWithArgumentsAnalzyer(
            [NotNull] string diagnosticId,
            [NotNull] string title,
            [NotNull] string message,
            [NotNull] string category,
            [NotNull] string description,
            DiagnosticSeverity diagnosticSeverity)
        {
            Rule = new DiagnosticDescriptor(diagnosticId, title, message, category, diagnosticSeverity, isEnabledByDefault: true, description: description);
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <summary>
        /// Gets the description of the Diagnostic Rule. Used when passing details to Roslyn.
        /// </summary>
        protected DiagnosticDescriptor Rule { get; }

        /// <summary>
        /// Gets the name of the method to check for.
        /// </summary>
        [NotNull]
        protected abstract string MethodName { get; }

        /// <summary>
        /// Gets the classes the method may belong to.
        /// </summary>
        [NotNull]
        protected abstract string[] ContainingTypes { get; }

        /// <inheritdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        /// <summary>
        /// Event for validating the arguments passed.
        /// </summary>
        /// <param name="context">The context for the Roslyn syntax analysis.</param>
        /// <param name="argumentList">Syntax representation of the argument list.</param>
        protected abstract void OnValidateArguments(SyntaxNodeAnalysisContext context, ArgumentListSyntax argumentList);

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
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
    }
}
