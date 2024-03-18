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

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.AspNetCore
{
    /// <summary>
    /// Analyzer for checking an API call uses ActionResult{T} instead of non-generic version, non-descript interfaces, or other return types.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ApiShouldUseGenericActionResultAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "ActionResult<TValue> should be used in an API.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiShouldUseGenericActionResultAnalyzer"/> class.
        /// </summary>
        public ApiShouldUseGenericActionResultAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.ApiShouldUseGenericActionResult,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: DiagnosticResultDescriptionFactory.ApiShouldUseGenericActionResult());
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclarationExpression, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethodDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            var methodDeclarationSyntax = (MethodDeclarationSyntax)node;

            // Check if the class is an API controller
            var classDeclarationSyntax = methodDeclarationSyntax.GetAncestor<ClassDeclarationSyntax>();
            if (classDeclarationSyntax == null)
            {
                return;
            }

            if (!classDeclarationSyntax.HasImplementedAnyOfType(
                    ["global::System.System.Web.Http.ApiController"],
                    null,
                    context.SemanticModel))
            {
                return;
            }

            // Check if the method is an API method
            foreach (var modifier in methodDeclarationSyntax.Modifiers)
            {
                var kind = modifier.Kind();

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (kind)
                {
                    case SyntaxKind.PublicKeyword:
                        break;
                    case SyntaxKind.StaticKeyword:
                        return;
                }
            }

            // Get the return type of the method
            var returnType = methodDeclarationSyntax.ReturnType;
            var typeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, returnType);

            if (typeInfo.Type == null)
            {
                return;
            }

            var typeFullName = typeInfo.Type.GetFullName();

            if (typeFullName.StartsWith("global::Microsoft.AspNetCore.Mvc.ActionResult<", StringComparison.Ordinal)
                || typeFullName.StartsWith("global::System.Threading.Task<global::Microsoft.AspNetCore.Mvc.ActionResult<", StringComparison.Ordinal))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, node.GetLocation()));
        }
    }
}
