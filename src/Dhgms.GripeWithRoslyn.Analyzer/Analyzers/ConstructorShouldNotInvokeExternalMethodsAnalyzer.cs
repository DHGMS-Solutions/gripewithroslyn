using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class ConstructorShouldNotInvokeExternalMethodsAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        internal const string Title = "ViewModel classes should inherit from a ViewModel interface.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "ViewModels should follow a consistent design of using ReactiveUI's ReactiveObject and an Interface";

        private const string GlobalSystemStringNamespace = "global::System.String";

        private readonly string[] operatorsWhiteList =
        {
            "nameof",
            "typeof"
        };

        private readonly (string Namespace, string[] Methods)[] _methodWhiteList =
        {
            (
                GlobalSystemStringNamespace,
                new []
                {
                    "Contains",
                    "EndsWith",
                    "Equals",
                    "IsNullOrWhiteSpace",
                    "IsNullOrEmpty",
                    "StartsWith",
                }),
        };

        /// <summary>
        /// Creates an instance of ConstructorShouldNotInvokeExternalMethodsAnalyzer
        /// </summary>
        public ConstructorShouldNotInvokeExternalMethodsAnalyzer()
        {
            this._rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.ConstructorShouldNotInvokeExternalMethods,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Description);
        }

        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this._rule);

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <remarks>
        /// https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        /// </remarks>
        /// <param name="context">
        /// Roslyn context.
        /// </param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated())
            {
                return;
            }

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

            context.ReportDiagnostic(Diagnostic.Create(this._rule, node.GetLocation()));
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
                    return operatorsWhiteList.Any(operatorName => operatorName.Equals(methodName, StringComparison.Ordinal));
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
                              && tuple.Methods.Any(tupleMethod => methodName.Equals(tupleMethod, StringComparison.Ordinal)));
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
    }
}
