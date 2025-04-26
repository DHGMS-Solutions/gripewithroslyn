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

        private static bool IsUsedInBaseConstructor(ConstructorDeclarationSyntax ctor, ParameterSyntax parameterSyntax)
        {
            // it's a constructor
            var initializer = ctor.Initializer;
            if (initializer != null)
            {
                // check if the argumentlist is using the parameter by name
                var argumentList = initializer.ArgumentList;
                if (argumentList != null)
                {
                    foreach (var argument in argumentList.Arguments)
                    {
                        if (argument.Expression is IdentifierNameSyntax identifierNameSyntax)
                        {
                            if (identifierNameSyntax.Identifier.Text.Equals(
                                    parameterSyntax.Identifier.Text,
                                    StringComparison.Ordinal))
                            {
                                return true;
                            }
                        }

                        if (argument.NameColon != null && argument.NameColon.Name.Identifier.Text == parameterSyntax.Identifier.Text)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
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
            if (!fullName.Equals("global::System.Object"))
            {
                return;
            }

            var parent = parameterSyntax.Parent;

            // Usually, the immediate parent is a ParameterListSyntax
            if (parent is not ParameterListSyntax parameterList)
            {
                return;
            }

            var methodOrCtor = parameterList.Parent;

            // This could be a MethodDeclarationSyntax, ConstructorDeclarationSyntax, DelegateDeclarationSyntax, etc.
            if (methodOrCtor is MethodDeclarationSyntax method)
            {
                if (method.IsDefinedByOverridenMethodOrInterface(semanticModel))
                {
                    return;
                }
            }
            else if (methodOrCtor is ConstructorDeclarationSyntax ctor)
            {
                if (IsUsedInBaseConstructor(ctor, parameterSyntax))
                {
                    return;
                }
            }
#if TBC
            else if (methodOrCtor is DelegateDeclarationSyntax @delegate)
            {
                // it's a delegate
            }
            else if (methodOrCtor is LocalFunctionStatementSyntax localFunction)
            {
                // local function inside a method
            }
#endif
            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(_rule, type.GetLocation()));
        }
    }
}
