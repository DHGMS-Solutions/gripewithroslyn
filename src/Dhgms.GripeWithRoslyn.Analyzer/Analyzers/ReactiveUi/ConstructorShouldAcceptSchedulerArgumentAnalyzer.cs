// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Dhgms.GripeWithRoslyn.Analyzer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi
{
    /// <summary>
    /// Analyzer for checking a constructor has a logging framework instance passed into it.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ConstructorShouldAcceptSchedulerArgumentAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "ViewModel Constructor should have accept Scheduler as a parameter.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorShouldAcceptSchedulerArgumentAnalyzer"/> class.
        /// </summary>
        public ConstructorShouldAcceptSchedulerArgumentAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.ConstructorShouldAcceptSchedulerArgument,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: DiagnosticResultDescriptionFactory.ConstructorShouldAcceptSchedulerArgument());
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.ConstructorDeclaration);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            var constructorDeclarationSyntax = (ConstructorDeclarationSyntax)context.Node;
            var classDeclarationSyntax = constructorDeclarationSyntax.GetAncestor<ClassDeclarationSyntax>();

            var baseClasses = new[]
            {
                "global::ReactiveUI.ReactiveObject"
            };

            var interfaces = Array.Empty<string>();

            if (!classDeclarationSyntax.HasImplementedAnyOfType(baseClasses, interfaces, context.SemanticModel))
            {
                return;
            }

            // check the parameters
            var parametersList = constructorDeclarationSyntax.ParameterList.Parameters;

            if (parametersList.Count == 0)
            {
                // no parameters
                LogWarning(context, node);
                return;
            }

            foreach (var parameter in parametersList)
            {
                var parameterType = parameter.Type;
                if (parameterType == null)
                {
                    // this is a problem, as we can't determine the type
                    LogWarning(context, node);
                    return;
                }

                var typeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, parameterType);
                var argType = typeInfo.Type;

                if (argType == null)
                {
                    // this is a problem, as we can't determine the type
                    LogWarning(context, node);
                    return;
                }

                var typeFullName = argType.GetFullName();
                if (typeFullName.Equals(
                        $"global::System.Reactive.Concurrency.Scheduler",
                        StringComparison.Ordinal))
                {
                    return;
                }
            }

            LogWarning(context, node);
        }

        private void LogWarning(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule, node.GetLocation()));
        }
    }
}
