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

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions
{
    /// <summary>
    /// Analyzer for ensuring that an inheriting type has a naming convention that ends with a specific suffix.
    /// </summary>
    public abstract class BaseClassInheritingTypeShouldEndWithSpecificSuffix : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClassInheritingTypeShouldEndWithSpecificSuffix"/> class.
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id.</param>
        /// <param name="title">The title of the analyzer.</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer.</param>
        /// <param name="diagnosticSeverity">The severity associated with breaches of the analyzer.</param>
        protected BaseClassInheritingTypeShouldEndWithSpecificSuffix(
            [NotNull] string diagnosticId,
            [NotNull] string title,
            [NotNull] string message,
            [NotNull] string category,
            [NotNull] string description,
            DiagnosticSeverity diagnosticSeverity)
        {
            _rule = new DiagnosticDescriptor(diagnosticId, title, message, category, diagnosticSeverity, isEnabledByDefault: true, description: description);
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <summary>
        /// Gets the name suffix that classes should end by.
        /// </summary>
        protected abstract string[] NameSuffixes { get; }

        /// <summary>
        /// Gets the full name of the base class that mean inheriting classes should use this rule.
        /// </summary>
        protected abstract string BaseClassFullName { get; }

        /// <inheritdoc />
        public sealed override void Initialize(AnalysisContext context)
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

            var identifier = classDeclarationSyntax.Identifier;
            if (NameSuffixes.Any(nameSuffix => identifier.Text.EndsWith(nameSuffix, StringComparison.OrdinalIgnoreCase)))
            {
                // it does end with the desired suffix
                return;
            }

            var baseList = classDeclarationSyntax.BaseList;

            if (baseList == null)
            {
                return;
            }

            if (baseList.Types.Count < 1)
            {
                return;
            }

            foreach (var baseTypeSyntax in baseList.Types)
            {
                var baseType = baseTypeSyntax.Type;
                var typeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, baseType);

                if (typeInfo.Type == null)
                {
                    return;
                }

                var typeFullName = typeInfo.Type.GetFullName();

                if (typeFullName.Equals(BaseClassFullName, StringComparison.Ordinal))
                {
                    context.ReportDiagnostic(Diagnostic.Create(_rule, identifier.GetLocation()));
                    return;
                }
            }
        }
    }
}
