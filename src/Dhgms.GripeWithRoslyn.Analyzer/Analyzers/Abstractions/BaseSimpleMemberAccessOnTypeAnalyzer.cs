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
    /// Abstract Analyzer for checking Simple Member Access on a Type.
    /// </summary>
    public abstract class BaseSimpleMemberAccessOnTypeAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSimpleMemberAccessOnTypeAnalyzer"/> class.
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id.</param>
        /// <param name="title">The title of the analyzer.</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer.</param>
        /// <param name="diagnosticSeverity">The severity associated with breaches of the analyzer.</param>
        protected BaseSimpleMemberAccessOnTypeAnalyzer(
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
        /// Gets the fully qualified class name to check for.
        /// </summary>
        protected abstract string ClassName { get; }

        /// <summary>
        /// Gets the Member name to check for.
        /// </summary>
        protected abstract string MemberName { get; }

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

            if (!typeFullName.Equals(ClassName, StringComparison.Ordinal))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, memberAccessExpressionSyntax.GetLocation()));
        }
    }
}
