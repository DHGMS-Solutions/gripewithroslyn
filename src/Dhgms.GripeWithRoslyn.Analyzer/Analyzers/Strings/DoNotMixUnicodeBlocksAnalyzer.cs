// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Unicode;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Strings
{
    /// <summary>
    /// Analyzer to check for mixed Unicode blocks in string literals.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoNotMixUnicodeBlocksAnalyzer : DiagnosticAnalyzer
    {
        private const string Title = "Mixed Unicode Blocks in String Literal";

        private const string MessageFormat = "String literal contains multiple Unicode blocks: {0}";

        private const string Category = SupportedCategories.Usage;

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotMixUnicodeBlocksAnalyzer"/> class.
        /// </summary>
        public DoNotMixUnicodeBlocksAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.DoNotMixUnicodeBlocks,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: DiagnosticResultDescriptionFactory.DoNotMixUnicodeBlocks());
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeStringLiteral, SyntaxKind.StringLiteralExpression);
        }

        private static HashSet<UnicodeRange> GetUnicodeBlockCount(string input)
        {
            var ranges = new[]
            {
                UnicodeRanges.BasicLatin,
                UnicodeRanges.Cyrillic,
                UnicodeRanges.Arabic,
                UnicodeRanges.Hebrew,
                UnicodeRanges.GreekandCoptic,
            };

            var usedRanges = new HashSet<UnicodeRange>();

            foreach (var currentChar in input)
            {
                foreach (var range in ranges)
                {
                    if (currentChar >= range.FirstCodePoint && currentChar <= range.FirstCodePoint + range.Length)
                    {
                        usedRanges.Add(range);
                        break;
                    }
                }
            }

            return usedRanges;
        }

        private void AnalyzeStringLiteral(SyntaxNodeAnalysisContext context)
        {
            var literal = (LiteralExpressionSyntax)context.Node;
            string value = literal.Token.ValueText;

            var blocks = GetUnicodeBlockCount(value);

            if (blocks.Count > 1)
            {
                var blockList = string.Join(", ", blocks);
                var diagnostic = Diagnostic.Create(_rule, literal.GetLocation(), blockList);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
