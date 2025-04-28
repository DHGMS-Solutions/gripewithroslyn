// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Dhgms.GripeWithRoslyn.Analyzer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Analyzer to check where a method uses return yield has the name starts with Enumerate.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class MethodsThatUseReturnYieldShouldHaveNameThatBeginsWithEnumerateAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Methods that use \"yield return\" should start with the name \"Enumerate\".";
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodsThatUseReturnYieldShouldHaveNameThatBeginsWithEnumerateAnalyzer"/> class.
        /// </summary>
        public MethodsThatUseReturnYieldShouldHaveNameThatBeginsWithEnumerateAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.MethodsThatUseReturnYieldShouldHaveNameThatBeginsWithEnumerate,
                Title,
                Title,
                SupportedCategories.Naming,
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
            context.RegisterSyntaxNodeAction(AnalyzeYieldReturnStatementExpression, SyntaxKind.YieldReturnStatement);
        }

        private void AnalyzeYieldReturnStatementExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not YieldStatementSyntax yieldStatementSyntax)
            {
                return;
            }

            if (!yieldStatementSyntax.ReturnOrBreakKeyword.IsKind(SyntaxKind.ReturnKeyword))
            {
                return;
            }

            var methodDeclarationSyntax = yieldStatementSyntax.GetAncestor<MethodDeclarationSyntax>();
            if (methodDeclarationSyntax == null)
            {
                return;
            }

            var semanticModel = context.SemanticModel;
            if (methodDeclarationSyntax.IsDefinedByOverridenMethodOrInterface(semanticModel))
            {
                return;
            }

            var identifier = methodDeclarationSyntax.Identifier;
            if (identifier.Text.StartsWith("Enumerate", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, identifier.GetLocation()));
        }
    }
}