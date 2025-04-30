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
        private const string GlobalSystemIObservableNamespace = "global::System.IObservable";
        private const string GlobalMicrosoftExtensionsLoggingLoggerMessage = "global::Microsoft.Extensions.Logging.LoggerMessage";

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
                new[]
                {
                    "Events"
                }),
            (
                GlobalSystemIObservableNamespace,
                new[]
                {
                    "Subscribe"
                }),
            (
                GlobalMicrosoftExtensionsLoggingLoggerMessage,
                new[]
                {
                    "Define"
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
                        or PropertyDeclarationSyntax
                        or FieldDeclarationSyntax
                        or ClassDeclarationSyntax
                        or SimpleLambdaExpressionSyntax
                        or ParenthesizedLambdaExpressionSyntax)
                {
                    // short circuit out
                    break;
                }

                currentNode = currentNode.Parent;
            }

            return null;
        }

        private static bool IsPrivateMethodOnSameClass(InvocationExpressionSyntax invocation, SemanticModel semanticModel, ClassDeclarationSyntax classDeclaration)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(invocation);
            if (symbolInfo.Symbol is not IMethodSymbol methodSymbol)
            {
                return false;
            }

            if (methodSymbol.DeclaredAccessibility != Accessibility.Private)
            {
                return false;
            }

            // Step 3: Get the enclosing class of the invocation
            var enclosingClass = invocation.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            if (enclosingClass == null)
            {
                return false;
            }

            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
            if (classSymbol == null)
            {
                return false;
            }

            // Step 5: Check if the method's containing type is the same as the enclosing class
            return SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType, classSymbol);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var parentMethodDeclaration = GetConstructorDeclarationSyntax(invocationExpression);

            if (parentMethodDeclaration == null)
            {
                return;
            }

            var classDeclaration = parentMethodDeclaration.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            if (IsPrivateMethodOnSameClass(invocationExpression, context.SemanticModel, classDeclaration))
            {
                return;
            }

            if (GetInheritsFromBaseClassThatShouldBeIgnored(context, invocationExpression))
            {
                return;
            }

            if (GetIsWhitelistedMethod(context, invocationExpression))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, node.GetLocation()));
        }

        private bool GetInheritsFromBaseClassThatShouldBeIgnored(
            SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocationExpression)
        {
            var classDeclarationSyntax = invocationExpression.GetAncestor<ClassDeclarationSyntax>();
            if (classDeclarationSyntax == null)
            {
                // not a class
                // probably a struct
                return false;
            }

            var baseClasses = new[]
            {
                "global::Xunit.TheoryData"
            };

            var interfaces = Array.Empty<string>();

            if (classDeclarationSyntax.HasImplementedAnyOfType(baseClasses, interfaces, context.SemanticModel))
            {
                return true;
            }

            return false;
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
