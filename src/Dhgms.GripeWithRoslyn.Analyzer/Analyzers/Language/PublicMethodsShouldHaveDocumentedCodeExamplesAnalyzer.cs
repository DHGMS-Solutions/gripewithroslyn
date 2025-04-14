// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Data;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Analyzer to check if an XMLDOC comment on a public method contains a code example.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PublicMethodsShouldHaveDocumentedCodeExamplesAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = SupportedCategories.Usage;
        private static readonly LocalizableString Title = "Public methods should have XML Documentation that contains an <example /> block.";
        private static readonly LocalizableString MessageFormat = "Method '{0}' is missing an XML Documentation <example /> element.";
        private static readonly LocalizableString Description = "Provide an <example /> in your XML Documentation to aid developers making use of the code.";

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicMethodsShouldHaveDocumentedCodeExamplesAnalyzer"/> class.
        /// </summary>
        public PublicMethodsShouldHaveDocumentedCodeExamplesAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.PublicMethodsShouldHaveDocumentedCodeExamples,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Description);
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
        }

        private void AnalyzeMethod(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;

            if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
            {
                return;
            }

            // Get XML documentation of the method
            var xmlDocumentation = methodSymbol.GetDocumentationCommentXml();

            if (!string.IsNullOrEmpty(xmlDocumentation))
            {
                // Look for the <example> tag in the XML doc
                var hasExampleTag = xmlDocumentation.Contains("<example>");
                if (hasExampleTag)
                {
                    return;
                }
            }

            // Report a diagnostic that no <example> exists
            var diagnostic = Diagnostic.Create(_rule, methodSymbol.Locations[0]);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
