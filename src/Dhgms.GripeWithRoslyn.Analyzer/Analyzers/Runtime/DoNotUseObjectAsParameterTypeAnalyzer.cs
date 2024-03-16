﻿// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime
{
    /// <summary>
    /// Analyzer to ensure <see cref="object"/> is not used in a parameter declaration.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseObjectAsParameterTypeAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Do not use Object in a parameter declaration.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseObjectAsParameterTypeAnalyzer"/> class.
        /// </summary>
        public DoNotUseObjectAsParameterTypeAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.DoNotUseObjectAsParameterType,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: DiagnosticResultDescriptionFactory.DoNotUseObjectAsParameterType());
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.Parameter);
        }

        private void AnalyzeParameter(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            var parameterSyntax = (ParameterSyntax)syntaxNodeAnalysisContext.Node;

            var semanticModel = syntaxNodeAnalysisContext.SemanticModel;
            var type = parameterSyntax.Type;
            if (type == null)
            {
                return;
            }

            var baseTypeInfo = semanticModel.GetTypeInfo(type);
            var baseTypeSymbol = baseTypeInfo.Type;

            if (baseTypeSymbol == null)
            {
                return;
            }

            var fullName = baseTypeSymbol.GetFullName(true);
            if (fullName == "global::System.Object")
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(_rule, type.GetLocation()));
            }
        }
    }
}
