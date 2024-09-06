// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Data;
using System.Linq;
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
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static bool IsDbSetMemberAccess(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccessExpr)
        {
            var semanticModel = context.SemanticModel;

            // Check if the left side of the member access is a property access (e.g., dbContext.Users)
            if (memberAccessExpr.Expression is IdentifierNameSyntax)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(memberAccessExpr);
                var symbol = symbolInfo.Symbol as IPropertySymbol;

                // Ensure that the property type is DbSet<TEntity>
                if (symbol != null && IsDbSet(symbol.Type))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsDbSet(ITypeSymbol typeSymbol)
        {
            // Check if the type is a DbSet<TEntity>
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                return namedTypeSymbol.ConstructedFrom.ToString() == "Microsoft.EntityFrameworkCore.DbSet<TEntity>";
            }

            return false;
        }

        private static bool QueryHasSelectClause(SyntaxNode node)
        {
            // Traverse the query chain to check for a .Select() method call
            while (node != null)
            {
                if (node is InvocationExpressionSyntax invocation)
                {
                    var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
                    if (memberAccess != null && memberAccess.Name.Identifier.Text.Equals("Select"))
                    {
                        return true; // .Select() projection found
                    }
                }

                node = node.Parent;
            }

            return false; // No .Select() found
        }

        private static bool IsEntityTypeReturn(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccessExpr)
        {
            var semanticModel = context.SemanticModel;

            // Get the type information of the expression
            var typeInfo = semanticModel.GetTypeInfo(memberAccessExpr);
            var returnType = typeInfo.Type;

            if (returnType == null || !(returnType is INamedTypeSymbol namedReturnType))
            {
                return false;
            }

            // Check if the return type is DbSet<TEntity>
            if (IsDbSet(namedReturnType))
            {
                var entityType = namedReturnType.TypeArguments.FirstOrDefault();
                if (entityType == null)
                {
                    return false;
                }

                // Check if the return type is the entity type itself (like IQueryable<TEntity>)
                if (namedReturnType.ConstructedFrom.ToString() == "System.Linq.IQueryable" &&
                    namedReturnType.TypeArguments.FirstOrDefault()?.Equals(entityType, SymbolEqualityComparer.Default) == true)
                {
                    return true;
                }

                // Check if the return type is Task<TEntity> (like async calls)
                if (namedReturnType.ConstructedFrom.ToString() == "System.Threading.Tasks.Task" &&
                    namedReturnType.TypeArguments.FirstOrDefault()?.Equals(entityType, SymbolEqualityComparer.Default) == true)
                {
                    return true;
                }

                // Check if the return type is a generic type that contains TEntity (like List<TEntity>)
                if (namedReturnType.TypeArguments.Any(t => t.Equals(entityType, SymbolEqualityComparer.Default)))
                {
                    return true;
                }
            }

            return false;
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var memberAccessExpr = (MemberAccessExpressionSyntax)context.Node;

            // Check if the expression starts with accessing a DbSet<TEntity>
            if (!IsDbSetMemberAccess(context, memberAccessExpr))
            {
                return;
            }

            // Walk through the fluent chain and check if it includes a .Select() projection
            if (!QueryHasSelectClause(memberAccessExpr))
            {
                // If there's no .Select() clause, analyze the final return type
                if (IsEntityTypeReturn(context, memberAccessExpr))
                {
                    var diagnostic = Diagnostic.Create(_rule, memberAccessExpr.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
   }
}