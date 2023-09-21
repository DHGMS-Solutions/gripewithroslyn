// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Analyzer for checking a constructor does not invoke external methods.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class ConstructorShouldNotInvokeExternalMethodsAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Constructors should minimise work and not execute methods";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "Constructors should minimise work and not execute methods. This is to make code easier to test, avoid performance risks, race conditions and quirks of the IDE designer.";

        private const string GlobalSystemStringNamespace = "global::System.String";
        private const string GlobalSystemArgumentNullExceptionNamespace = "global::System.ArgumentNullException";
        private const string GlobalReactiveMarblesObservableEventsNamespace = "global::ReactiveMarbles.ObservableEvents.ObservableGeneratorExtensions";
        private const string GlobalSystemIObservableNamespace = "global::System.IObservable<T>";

        private readonly DiagnosticDescriptor _rule;

        private readonly string[] _operatorsWhiteList =
        {
            "nameof",
            "typeof"
        };

        private readonly (string Namespace, string[] Methods)[] _methodWhiteList =
        {
            (
                GlobalSystemStringNamespace,
                new[]
                {
                    "Contains",
                    "EndsWith",
                    "Equals",
                    "IsNullOrWhiteSpace",
                    "IsNullOrEmpty",
                    "StartsWith",
                }),
            (
                GlobalSystemArgumentNullExceptionNamespace,
                new[]
                {
                    "ThrowIfNull"
                }),
            (
                GlobalReactiveMarblesObservableEventsNamespace,
                Array.Empty<string>()),
            (
                GlobalSystemIObservableNamespace,
                new[]
                {
                    "Subscribe"
                }),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorShouldNotInvokeExternalMethodsAnalyzer"/> class.
        /// </summary>
        public ConstructorShouldNotInvokeExternalMethodsAnalyzer()
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
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private static ConstructorDeclarationSyntax GetConstructorDeclarationSyntax(SyntaxNode syntaxNode)
        {
            var currentNode = syntaxNode.Parent;
            while (currentNode != null)
            {
                if (currentNode is ConstructorDeclarationSyntax constructorDeclarationSyntax)
                {
                    return constructorDeclarationSyntax;
                }

                if (currentNode is MethodDeclarationSyntax
                    || currentNode is PropertyDeclarationSyntax
                    || currentNode is FieldDeclarationSyntax
                    || currentNode is ClassDeclarationSyntax)
                {
                    // short circuit out
                    break;
                }

                currentNode = currentNode.Parent;
            }

            return null;
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (GetIsWhitelistedMethod(context, invocationExpression))
            {
                return;
            }

            var parentMethodDeclaration = GetConstructorDeclarationSyntax(invocationExpression);

            if (parentMethodDeclaration == null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, node.GetLocation()));
        }

        private bool GetIsWhitelistedMethod(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression)
        {
            switch (invocationExpression.Expression)
            {
                case MemberAccessExpressionSyntax memberAccessExpression:
                    return GetIsWhiteListedMemberAccessMethod(
                        context,
                        memberAccessExpression);
                case IdentifierNameSyntax identifierNameSyntax:
                    {
                        var methodName = identifierNameSyntax.Identifier.ToFullString();
                        return _operatorsWhiteList.Any(operatorName => operatorName.Equals(methodName, StringComparison.Ordinal));
                    }

                default:
                    return false;
            }
        }

        private bool GetIsWhiteListedMemberAccessMethod(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccessExpression)
        {
            var typeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, memberAccessExpression.Expression);
            if (typeInfo.Type == null)
            {
                return false;
            }

            var typeFullName = typeInfo.Type.GetFullName();

            var methodName = memberAccessExpression.Name.ToString();

            return _methodWhiteList
                .Any(tuple => tuple.Namespace.Equals(typeFullName)
                              && (
                                  tuple.Methods.Length == 0
                                  || tuple.Methods.Any(tupleMethod => methodName.Equals(tupleMethod, StringComparison.Ordinal))));
        }
    }
}
