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
    /// Analyzer to check if a class with the abstract keyword starts with the word "Abstract".
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class ClassWithAbstractKeywordAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Class with abstract keyword should start with Abstract";
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassWithAbstractKeywordAnalyzer"/> class.
        /// </summary>
        public ClassWithAbstractKeywordAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.ClassWithAbstractKeyword,
                "Class with abstract keyword should start with Abstract",
                "Class with abstract keyword should start with Abstract",
                SupportedCategories.Naming,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: "Class with abstract keyword should start with Abstract");
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclarationExpression, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeClassDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ClassDeclarationSyntax classDeclarationSyntax))
            {
                return;
            }

            if (!classDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                return;
            }

            var identifier = classDeclarationSyntax.Identifier;
            if (identifier.Text.StartsWith("Abstract", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, identifier.GetLocation()));
        }
    }
}
