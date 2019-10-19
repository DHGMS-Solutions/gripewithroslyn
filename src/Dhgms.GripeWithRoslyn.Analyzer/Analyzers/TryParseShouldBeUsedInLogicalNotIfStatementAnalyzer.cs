using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class TryParseShouldBeUsedInLogicalNotIfStatementAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        internal const string Title = "Constructors should minimise work and not execute methods";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "Constructors should minimise work and not execute methods. This is due make code easier to test, poor performance, race conditions and quirks of IDE designer.";

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
        public TryParseShouldBeUsedInLogicalNotIfStatementAnalyzer()
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

            if (!(invocationExpression.Expression is IdentifierNameSyntax identifierNameSyntax))
            {
                return;
            }

            if (!identifierNameSyntax.Identifier.Text.Equals("TryParse", StringComparison.Ordinal))
            {
                return;
            }

            if (IsInIfStatementWithLogicalNotOperation(invocationExpression))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(this._rule, node.GetLocation()));
        }

        private bool IsInIfStatementWithLogicalNotOperation(SyntaxNode syntaxNode)
        {
            var prefixUnaryExpressionSyntax = GetParentNodeOfType<PrefixUnaryExpressionSyntax>(syntaxNode);

            if (prefixUnaryExpressionSyntax == null)
            {
                return false;
            }

            var ifStatementSyntax = GetParentNodeOfType<PrefixUnaryExpressionSyntax>(prefixUnaryExpressionSyntax);

            return ifStatementSyntax != null;
        }

        private static T GetParentNodeOfType<T>(SyntaxNode syntaxNode)
            where T : SyntaxNode
        {
            var currentNode = syntaxNode.Parent;
            while (currentNode != null)
            {

                if (currentNode is T parent)
                {
                    return parent;
                }

                currentNode = currentNode.Parent;
            }

            return null;
        }
    }
}