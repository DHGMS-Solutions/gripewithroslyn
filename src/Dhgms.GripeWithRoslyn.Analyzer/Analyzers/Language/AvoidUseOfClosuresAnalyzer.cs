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
    /// Analyzer for warning on usages of closures in lambdas.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AvoidUseOfClosuresAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Review use of closure";
        internal const string Message = "Review use of closure: '{0}'";
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvoidUseOfClosuresAnalyzer"/> class.
        /// </summary>
        public AvoidUseOfClosuresAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.AvoidUseOfClosures,
                Title,
                Message,
                SupportedCategories.Performance,
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

            context.RegisterSyntaxNodeAction(
                AnalyzeLambdaExpression,
                SyntaxKind.SimpleLambdaExpression,
                SyntaxKind.ParenthesizedLambdaExpression);
        }

        private void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not LambdaExpressionSyntax lambdaExpression)
            {
                return;
            }

            // Get the semantic model
            var semanticModel = context.SemanticModel;

            // Analyze the data flow within the lambda expression
            var dataFlowAnalysis = semanticModel.AnalyzeDataFlow(lambdaExpression);

            if (dataFlowAnalysis == null)
            {
                return;
            }

            // The Captured collection contains the variables that are captured by the lambda
            foreach (var capturedSymbol in dataFlowAnalysis.Captured)
            {
                // Report a diagnostic if a closure is detected
                var diagnostic = Diagnostic.Create(
                    _rule,
                    lambdaExpression.GetLocation(),
                    capturedSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
