// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Dhgms.GripeWithRoslyn.Analyzer.Extensions;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions
{
    /// <summary>
    /// Abstraction for an analyzer that checks the type argument for a generic inheritance should end with a specific suffix.
    /// </summary>
    public abstract class AbstractTypeArgumentForClassShouldEndWithSpecificSuffixAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTypeArgumentForClassShouldEndWithSpecificSuffixAnalyzer"/> class.
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id.</param>
        /// <param name="title">The title of the analyzer.</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer.</param>
        /// <param name="diagnosticSeverity">The severity associated with breaches of the analyzer.</param>
        protected AbstractTypeArgumentForClassShouldEndWithSpecificSuffixAnalyzer(
            [NotNull] string diagnosticId,
            [NotNull] string title,
            [NotNull] string message,
            [NotNull] string category,
            [NotNull] string description,
            DiagnosticSeverity diagnosticSeverity)
        {
            _rule = new DiagnosticDescriptor(diagnosticId, title, message, category, diagnosticSeverity, isEnabledByDefault: true, description: description);
        }

        /// <summary>
        /// Gets a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <summary>
        /// Gets the suffix of the class to check for.
        /// </summary>
        [NotNull]
        protected abstract string[] ClassNameSuffixes { get; }

        /// <summary>
        /// Gets the containing types the method may belong to.
        /// </summary>
        [NotNull]
        protected abstract string BaseClassFullName { get; }

        /// <inheritdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeTypeArgumentListExpression, SyntaxKind.TypeArgumentList);
        }

        private void AnalyzeTypeArgumentListExpression(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is TypeArgumentListSyntax typeArgumentListSyntax))
            {
                return;
            }

            var baseList = typeArgumentListSyntax.GetAncestor<BaseListSyntax>();

            var hasMatch = false;
            if (baseList != null
                && baseList.Types.Count > 0)
            {
                foreach (var baseTypeSyntax in baseList.Types)
                {
                    var baseType = baseTypeSyntax.Type;
                    var typeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, baseType);

                    if (typeInfo.Type == null)
                    {
                        continue;
                    }

                    var typeFullName = typeInfo.Type.GetFullName();

                    if (typeFullName.Equals(BaseClassFullName, StringComparison.Ordinal))
                    {
                        hasMatch = true;
                        break;
                    }
                }
            }

            if (!hasMatch)
            {
                return;
            }

            var arguments = typeArgumentListSyntax.Arguments;
            if (arguments.Count != 1)
            {
                // we may need to adjust this in future
                // where another class could be hiding IRequest and have multiple arguments.
                return;
            }

            var argument = arguments.First();
            SyntaxToken? syntaxToken = null;
            if (argument is IdentifierNameSyntax identifierNameSyntax)
            {
                syntaxToken = identifierNameSyntax.Identifier;
            }
            else if (argument is PredefinedTypeSyntax predefinedTypeSyntax)
            {
                syntaxToken = predefinedTypeSyntax.Keyword;
            }

            if (syntaxToken != null)
            {
                if (ClassNameSuffixes.Any(suffix => syntaxToken.Value.Text.EndsWith(suffix)))
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, argument.GetLocation()));
        }
    }
}
