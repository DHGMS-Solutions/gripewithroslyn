// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions
{
    /// <summary>
    /// Base class for checking if a method invocation is from a specific class.
    /// </summary>
    public abstract class BaseInvocationUsingClassAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseInvocationUsingClassAnalyzer"/> class.
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id.</param>
        /// <param name="title">The title of the analyzer.</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer.</param>
        /// <param name="diagnosticSeverity">The severity associated with breaches of the analyzer.</param>
        protected BaseInvocationUsingClassAnalyzer(
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
        /// Gets the class name the rule should check for.
        /// </summary>
        protected abstract string ClassName { get; }

        /// <inheritdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)context.Node;

            if (memberAccessExpressionSyntax == null
                || memberAccessExpressionSyntax.Expression == null)
            {
                return;
            }

            var typeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, memberAccessExpressionSyntax.Expression);

            if (typeInfo.Type == null)
            {
                return;
            }

            var typeFullName = typeInfo.Type.GetFullName();

            if (!typeFullName.Equals(ClassName, StringComparison.Ordinal))
            {
                // need to look at if it's a static member access if we get here.
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, memberAccessExpressionSyntax.GetLocation()));
        }
    }
}
