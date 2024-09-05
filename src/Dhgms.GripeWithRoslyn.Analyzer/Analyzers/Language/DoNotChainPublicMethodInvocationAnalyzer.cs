// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Analyzer for warning on where a public method invokes another public method on the same class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoNotChainPublicMethodInvocationAnalyzer : DiagnosticAnalyzer
    {
        private const string Title = "Public method invocation";
        private const string MessageFormat = "Public method '{0}' calls another public method '{1}' in the same class";
        private const string Description = "Public methods should not call other public methods in the same class";

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotChainPublicMethodInvocationAnalyzer"/> class.
        /// </summary>
        public DoNotChainPublicMethodInvocationAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.DoNotChainPublicMethodInvocation,
                Title,
                MessageFormat,
                SupportedCategories.Design,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Description);
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // Register a syntax node action for method invocations
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethodInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeMethodInvocation(SyntaxNodeAnalysisContext context)
        {
            // Get the invocation expression node
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            // Get the method being invoked
            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocationExpression);

            if (symbolInfo.Symbol is not IMethodSymbol invokedMethod)
            {
                return;
            }

            // Check if the invoked method is public and is in the same class
            var containingClass = invokedMethod.ContainingType;
            if (invokedMethod.DeclaredAccessibility != Accessibility.Public || containingClass == null)
            {
                return;
            }

            // Check if the calling method is also public
            var methodDeclaration = invocationExpression.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (methodDeclaration == null)
            {
                return;
            }

            if (context.SemanticModel.GetDeclaredSymbol(methodDeclaration) is not { DeclaredAccessibility: Accessibility.Public } callingMethod ||
                !callingMethod.ContainingType.Equals(invokedMethod.ContainingType, SymbolEqualityComparer.Default))
            {
                return;
            }

            // Create and report a diagnostic
            var diagnostic = Diagnostic.Create(
                _rule,
                invocationExpression.GetLocation(),
                callingMethod.Name,
                invokedMethod.Name);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
