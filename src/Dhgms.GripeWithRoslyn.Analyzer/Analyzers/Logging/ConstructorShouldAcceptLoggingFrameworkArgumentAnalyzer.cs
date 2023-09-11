// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Dhgms.GripeWithRoslyn.Analyzer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Logging
{
    /// <summary>
    /// Analyzer for checking a constructor has a logging framework instance passed into it.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ConstructorShouldAcceptLoggingFrameworkArgumentAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Constructors should minimise work and not execute methods";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "Constructors should minimise work and not execute methods. This is to make code easier to test, avoid performance risks, race conditions and quirks of the IDE designer.";

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorShouldAcceptLoggingFrameworkArgumentAnalyzer"/> class.
        /// </summary>
        public ConstructorShouldAcceptLoggingFrameworkArgumentAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.ConstructorShouldNotInvokeExternalMethods,
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
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.ConstructorDeclaration);
        }

        private static string GetFullName(ConstructorDeclarationSyntax constructorDeclarationSyntax)
        {
            var classDeclarationSyntax = constructorDeclarationSyntax.GetAncestor<ClassDeclarationSyntax>();
            var namespaceDeclarationSyntax = constructorDeclarationSyntax.GetAncestor<NamespaceDeclarationSyntax>();

            var namespaceName = namespaceDeclarationSyntax.Name.ToString();
            return namespaceName + "." + classDeclarationSyntax.Identifier.ToString();
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            var constructorDeclarationSyntax = (ConstructorDeclarationSyntax)context.Node;
            var myType = GetFullName(constructorDeclarationSyntax);

            // check the parameters
            var parametersList = constructorDeclarationSyntax.ParameterList.Parameters;

            if (parametersList.Count == 0)
            {
                // no parameters, so no logging framework
            }
            else
            {
                var lastParameter = parametersList.Last();
                var lastParameterType = lastParameter.Type;
                if (lastParameterType == null)
                {
                    // no op, as still spiking this out
                }
                else
                {
                    var typeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, lastParameterType);

                    if (typeInfo.Type == null)
                    {
                        // no op, as still spiking this out
                    }
                    else
                    {
                        // TODO: GetFullName doesn't handle generic arguments.
                        var typeFullName = typeInfo.Type.GetFullName();
                        if (typeFullName.Equals(
                                $"global::Microsoft.Extensions.Logging.ILogger<{myType}>",
                                StringComparison.Ordinal))
                        {
                            return;
                        }
                    }
                }
            }

            // get those that are logging framework types
            // on a count of 0, report a diagnostic
            // on a count of 1, check the name is relevant to the type "logger" and the type is ILogger<T>
            // on 2 check we go LogMessageActions then ILogger<T>
            context.ReportDiagnostic(Diagnostic.Create(_rule, node.GetLocation()));
        }
    }
}
