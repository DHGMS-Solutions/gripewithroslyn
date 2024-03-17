// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Analyzer to warn about the use of manual event subscriptions.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class DoNotUseManualEventSubscriptionsAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Do not use manual event subscriptions. Consider a ReactiveMarbles ObservableEvents approach.";
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseManualEventSubscriptionsAnalyzer"/> class.
        /// </summary>
        public DoNotUseManualEventSubscriptionsAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.DoNotUseManualEventSubscriptions,
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
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclarationExpression, SyntaxKind.AddAssignmentExpression);
        }

        private void AnalyzeClassDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not AssignmentExpressionSyntax assignmentExpressionSyntax)
            {
                return;
            }

            var assignmentTarget = assignmentExpressionSyntax.Left;
            var assignmentTargetSymbol = context.SemanticModel.GetSymbolInfo(assignmentTarget).Symbol;

            if (assignmentTargetSymbol is not IEventSymbol _)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, assignmentExpressionSyntax.GetLocation()));
        }
    }
}