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
    /// Analyzer to ensure Tuples are not used.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseTuplesAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Do not use Tuples.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseTuplesAnalyzer"/> class.
        /// </summary>
        public DoNotUseTuplesAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.DoNotUseTuples,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: DiagnosticResultDescriptionFactory.DoNotUseTuples());
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.TupleElement, SyntaxKind.TupleExpression, SyntaxKind.TupleType);
        }

        private void AnalyzeParameter(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            switch (syntaxNodeAnalysisContext.Node)
            {
                case TupleElementSyntax tupleElementSyntax:
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(_rule, tupleElementSyntax.GetLocation()));
                    break;
                case TupleExpressionSyntax tupleExpressionSyntax:
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(_rule, tupleExpressionSyntax.GetLocation()));
                    break;
                case TupleTypeSyntax tupleTypeSyntax:
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(_rule, tupleTypeSyntax.GetLocation()));
                    break;
            }
        }
    }
}
