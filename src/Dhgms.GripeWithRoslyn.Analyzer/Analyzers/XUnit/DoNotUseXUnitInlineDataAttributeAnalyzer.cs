// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.XUnit
{
    /// <summary>
    /// Analyzer for detecting the use of XUnit's InlineData attribute.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseXUnitInlineDataAttributeAnalyzer : DiagnosticAnalyzer
    {
        private const string Title = "Do not use the XUnit Attribute InlineData";

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseXUnitInlineDataAttributeAnalyzer"/> class.
        /// </summary>
        public DoNotUseXUnitInlineDataAttributeAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.DoNotUseXUnitInlineDataAttribute,
                Title,
                Title,
                SupportedCategories.Maintainability,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                "InlineData has maintainability and sustainability issues. It is better to use ClassData that points to a class inheriting from TheoryData.");
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
        }

        private void AnalyzeAttribute(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            var attributeSyntax = (AttributeSyntax)syntaxNodeAnalysisContext.Node;

            var typeInfo = ModelExtensions.GetTypeInfo(syntaxNodeAnalysisContext.SemanticModel, attributeSyntax);

            var fullName = typeInfo.Type.GetFullName(true);
            if (fullName == null || !fullName.Equals("global::XUnit.InlineDataAttribute"))
            {
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(_rule, syntaxNodeAnalysisContext.Node.GetLocation()));
        }
    }
}