﻿// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    /// <summary>
    /// Abstract Implementation for an analyzer that ensures a type ends with a specific suffix.
    /// </summary>
    public abstract class BaseInterfaceInheritingTypeShouldEndWithSpecificSuffix : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseInterfaceInheritingTypeShouldEndWithSpecificSuffix"/> class.
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id.</param>
        /// <param name="title">The title of the analyzer.</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer.</param>
        /// <param name="diagnosticSeverity">The severity associated with breaches of the analyzer.</param>
        protected BaseInterfaceInheritingTypeShouldEndWithSpecificSuffix(
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
        /// Gets the name suffix for the name.
        /// </summary>
        protected abstract string NameSuffix { get; }

        /// <summary>
        /// Gets the full name of the base class this rule applies to.
        /// </summary>
        protected abstract string BaseClassFullName { get; }

        /// <inheritdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeInterfaceDeclarationExpression, SyntaxKind.InterfaceDeclaration);
        }

        private void AnalyzeInterfaceDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated())
            {
                return;
            }

            var interfaceDeclarationSyntax = context.Node as InterfaceDeclarationSyntax;

            if (interfaceDeclarationSyntax == null)
            {
                return;
            }

            var identifier = interfaceDeclarationSyntax.Identifier;
            if (identifier.Text.EndsWith(NameSuffix, StringComparison.OrdinalIgnoreCase))
            {
                // it does end with the desired suffix
                // no point checking to warn if it should or not.
                // that's not the point of this validator.
                return;
            }

            var baseList = interfaceDeclarationSyntax.BaseList;

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
