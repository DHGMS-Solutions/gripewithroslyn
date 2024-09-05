// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.EfCore
{
    /// <summary>
    /// Analyzer to suggest using projections in EF Core queries.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class QueriesShouldUseProjectionsAnalyzer : DiagnosticAnalyzer
    {
        private const string Title = "EF Core query should use projections";
        private const string MessageFormat = "EF Core query is missing a projection";
        private const string Description = "Consider using a projection (e.g., Select with specific fields) to avoid loading unnecessary data.";

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueriesShouldUseProjectionsAnalyzer"/> class.
        /// </summary>
        public QueriesShouldUseProjectionsAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.EfCoreQueriesShouldUseProjections,
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
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static bool IsDbSet(ISymbol symbol)
        {
            // Ensure the symbol is an EF Core DbSet
            var type = symbol.ContainingType;
            if (type == null)
            {
                return false;
            }

            // Check if it's an instance of DbSet<TEntity>
            return type.ToString().Contains("Microsoft.EntityFrameworkCore.DbSet");
        }

        private static bool QueryHasSelectClause(SyntaxNode node)
        {
            // Traverse the query chain and check for a Select() invocation
            while (node != null)
            {
                if (node is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "Select" } })
                {
                    return true;  // Found a projection with Select()
                }

                node = node.Parent;
            }

            return false;  // No Select found in the query chain
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocationExpr = (InvocationExpressionSyntax)context.Node;

            // First, check if the expression is operating on IQueryable
            var memberAccessExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpr == null)
            {
                return;
            }

            // Check if the query is an IQueryable off of a DbSet (from DbContext)
            var semanticModel = context.SemanticModel;
            var symbolInfo = semanticModel.GetSymbolInfo(memberAccessExpr.Expression);
            var symbol = symbolInfo.Symbol;

            if (symbol == null)
            {
                return;
            }

            // Ensure the symbol is an EF Core DbSet
            if (!IsDbSet(symbol))
            {
                return;
            }

            // Check if the chain does NOT include a .Select() clause (missing projection)
            if (QueryHasSelectClause(invocationExpr))
            {
                return;
            }

            // If there's no .Select() clause, raise a diagnostic warning
            var diagnostic = Diagnostic.Create(_rule, invocationExpr.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}