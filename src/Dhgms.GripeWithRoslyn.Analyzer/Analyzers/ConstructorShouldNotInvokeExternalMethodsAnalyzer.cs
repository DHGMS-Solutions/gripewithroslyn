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

            // might want to skip through whitelist methods
            // such as string.IsNullOrWhitespace();

            var parentMethodDeclaration = GetConstructorDeclarationSyntax(context.Node);

            if (parentMethodDeclaration == null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(this._rule, node.GetLocation()));
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
