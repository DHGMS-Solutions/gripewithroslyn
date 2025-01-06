// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime
{
    /// <summary>
    /// Analyzer that checks for methods that have an overload that takes an IFileProvider.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseFileProviderOverloadAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = SupportedCategories.Design;
        private static readonly LocalizableString Title = "Method has an overload that takes IFileProvider";
        private static readonly LocalizableString MessageFormat = "Method '{0}' has an overload that takes IFileProvider";
        private static readonly LocalizableString Description = "This method has an overload that accepts an IFileProvider, use this to make your code easier to test.";

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="UseFileProviderOverloadAnalyzer"/> class.
        /// </summary>
        public UseFileProviderOverloadAnalyzer()
        {
            _rule = new DiagnosticDescriptor(DiagnosticIdsHelper.UseFileProviderOverload, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(
                AnalyzeInvocationExpression,
                SyntaxKind.InvocationExpression);
        }

        private static bool HasFileProviderArgument(IMethodSymbol methodSymbol)
        {
            return methodSymbol.Parameters.Any(p => p.Type.ToString() == "Microsoft.Extensions.FileProviders.IFileProvider");
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpr = (InvocationExpressionSyntax)context.Node;

            if (context.SemanticModel.GetSymbolInfo(invocationExpr).Symbol is not IMethodSymbol methodSymbol)
            {
                return;
            }

            if (HasFileProviderArgument(methodSymbol))
            {
                return;
            }

            var containingType = methodSymbol.ContainingType;
            var overloads = containingType.GetMembers().OfType<IMethodSymbol>()
                .Where(m => m.Name == methodSymbol.Name && HasFileProviderArgument(m));

            if (overloads.Any())
            {
                var diagnostic = Diagnostic.Create(_rule, invocationExpr.GetLocation(), methodSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
