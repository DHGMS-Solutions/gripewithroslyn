// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Analyzer to check if an XMLDOC comment on a public method contains a code example.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ProvideExampleInXmlDocAnalyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context)
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
            var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations[0]);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
