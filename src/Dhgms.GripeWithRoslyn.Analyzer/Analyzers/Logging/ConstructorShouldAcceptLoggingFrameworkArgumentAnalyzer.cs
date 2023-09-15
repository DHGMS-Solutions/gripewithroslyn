// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Linq;
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
        internal const string Title = "Constructor should have a logging framework instance as the final parameter.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorShouldAcceptLoggingFrameworkArgumentAnalyzer"/> class.
        /// </summary>
        public ConstructorShouldAcceptLoggingFrameworkArgumentAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.ConstructorShouldAcceptLoggingFrameworkArgument,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: DiagnosticResultDescriptionFactory.ConstructorShouldAcceptLoggingFrameworkArgument());
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

        private static string GetFullName(
            ConstructorDeclarationSyntax constructorDeclarationSyntax,
            ClassDeclarationSyntax classDeclarationSyntax)
        {
            var namespaceDeclarationSyntax = constructorDeclarationSyntax.GetAncestor<NamespaceDeclarationSyntax>();

            var namespaceName = namespaceDeclarationSyntax.Name.ToString();
            return $"global::{namespaceName}.{classDeclarationSyntax.Identifier}";
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            var constructorDeclarationSyntax = (ConstructorDeclarationSyntax)context.Node;
            var classDeclarationSyntax = constructorDeclarationSyntax.GetAncestor<ClassDeclarationSyntax>();
            var myType = GetFullName(constructorDeclarationSyntax, classDeclarationSyntax);

            // skip if the class implements ILogMessageActions<T> or ILogMessageActionsWrapper<T>
            var baseList = classDeclarationSyntax.BaseList;

            if (baseList != null
                && baseList.Types.Count > 0)
            {
                foreach (var baseTypeSyntax in baseList.Types)
                {
                    var baseType = baseTypeSyntax.Type;
                    var baseTypeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, baseType);

                    if (baseTypeInfo.Type == null)
                    {
                        continue;
                    }

                    var baseTypeFullName = baseTypeInfo.Type.GetFullName();

                    if (baseTypeFullName is "global::Whipstaff.Core.Logging.ILogMessageActions" or "global::Whipstaff.Core.Logging.ILogMessageActionsWrapper")
                    {
                        return;
                    }

                    if (baseTypeInfo.Type.AllInterfaces.Any(symbol =>
                        {
                            var fn = symbol.GetFullName();
                            return fn is "global::Whipstaff.Core.Logging.ILogMessageActions"
                                or "global::Whipstaff.Core.Logging.ILogMessageActionsWrapper";
                        }))
                    {
                        return;
                    }
                }
            }

            // we can also skip out if the class has no methods
            // as there won't be any logging going on.
            var methodDeclarationSyntaxes = classDeclarationSyntax.ChildNodes().OfType<MethodDeclarationSyntax>().ToArray();
            if (methodDeclarationSyntaxes.Length == 0)
            {
                return;
            }

            // check the parameters
            var parametersList = constructorDeclarationSyntax.ParameterList.Parameters;

            if (parametersList.Count == 0)
            {
                // no parameters, so no logging framework
                LogWarning(context, node);
                return;
            }

            var lastParameter = parametersList.Last();
            var lastParameterType = lastParameter.Type;
            if (lastParameterType == null)
            {
                // this is a problem, as we can't determine the type
                LogWarning(context, node);
                return;
            }

            var typeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, lastParameterType);
            var argType = typeInfo.Type;

            if (argType == null)
            {
                // this is a problem, as we can't determine the type
                LogWarning(context, node);
                return;
            }

            var typeFullName = argType.GetFullName();
            if (typeFullName.Equals(
                    $"global::Microsoft.Extensions.Logging.ILogger",
                    StringComparison.Ordinal))
            {
                CheckGenericArgument(context, lastParameter, node, myType);

                return;
            }

            // check if implementing Whipstaff.Core.Logging.ILogMessageActionsWrapper<T>
            var lastParameterAllInterfaces = argType.AllInterfaces;
            foreach (var namedTypeSymbol in lastParameterAllInterfaces)
            {
                var interfaceName = namedTypeSymbol.GetFullName();
                if (interfaceName.Equals(
                        $"global::Whipstaff.Core.Logging.ILogMessageActionsWrapper",
                        StringComparison.Ordinal)
                    && namedTypeSymbol.TypeArguments.Any(x => x.GetFullName().Equals(myType, StringComparison.Ordinal)))
                {
                    return;
                }
            }

            LogWarning(context, node);
        }

        private void CheckGenericArgument(
            SyntaxNodeAnalysisContext context,
            ParameterSyntax lastParameter,
            SyntaxNode node,
            string myType)
        {
            // var genericArgs = argType.GetGenericArguments();
            var childNodes = lastParameter.ChildNodes();

            // QualifiedNameSyntax
            var qualifiedNameSyntax = childNodes.OfType<QualifiedNameSyntax>().FirstOrDefault();

            // GenericNameSyntax
            var genericNameSyntax = qualifiedNameSyntax.ChildNodes().OfType<GenericNameSyntax>().ToArray();

            // GenericTokenSyntax
            var genericTokenSyntax = genericNameSyntax[0];

            // type arg list
            var typeArgumentList = genericTokenSyntax.TypeArgumentList;
            var typeArgumentListArgs = typeArgumentList.Arguments;

            // we should only have 1 arg for ILogger<T>.
            if (typeArgumentListArgs.Count != 1)
            {
                LogWarning(context, node);
                return;
            }

            var genericArgType = ModelExtensions.GetTypeInfo(context.SemanticModel, typeArgumentListArgs[0]);
            var genericArgTypeType = genericArgType.Type;
            if (genericArgTypeType == null)
            {
                // this is a problem, as we can't determine the type
                LogWarning(context, node);
                return;
            }

            var genericArgTypeTypeFullName = genericArgTypeType.GetFullName();
            if (!genericArgTypeTypeFullName.Equals(
                    myType,
                    StringComparison.Ordinal))
            {
                LogWarning(context, node);
            }
        }

        private void LogWarning(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule, node.GetLocation()));
        }
    }
}
