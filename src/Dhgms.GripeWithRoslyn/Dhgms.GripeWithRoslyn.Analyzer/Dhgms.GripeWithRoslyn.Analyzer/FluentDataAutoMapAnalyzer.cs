using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dhgms.GripeWithRoslyn.Analyzer
{
    using System.Collections.Immutable;
    using System.Diagnostics;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Roslyn Analyzer to check for uses of FluentData's AutoMap method
    /// </summary>
    /// <remarks>
    /// Based upon : https://raw.githubusercontent.com/Wintellect/Wintellect.Analyzers/master/Source/Wintellect.Analyzers/Wintellect.Analyzers/Usage/CallAssertMethodsWithMessageParameterAnalyzer.cs
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class FluentDataAutoMapAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DhgmsGripeWithRoslynAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            //if (context.IsGeneratedOrNonUserCode())
            //{
                //return;
            //}

            InvocationExpressionSyntax invocationExpr = context.Node as InvocationExpressionSyntax;
            if (invocationExpr == null)
            {
                return;
            }

            var expression = invocationExpr.Expression as MemberAccessExpressionSyntax;
            if (expression == null)
            {
                return;
            }

            var methodName = "AutoMap";
            var classNames = new[]
                {
                    "InsertBuilder",
                    "StoredProcedureBuilder",
                    "UpdateBuilder"
                };

            if (!expression.Name.Identifier.ValueText.Equals(methodName, StringComparison.Ordinal))
            {
                return;
            }

            IMethodSymbol memberSymbol = context.SemanticModel.GetSymbolInfo(invocationExpr).Symbol as IMethodSymbol;
            if (memberSymbol != null)
            {
                INamedTypeSymbol classSymbol = memberSymbol.ContainingSymbol as INamedTypeSymbol;
                if (classSymbol != null)
                {
                    if (classNames.All(c => !classSymbol.Name.Equals(c, StringComparison.Ordinal)))
                    {
                        return;
                    }
                }
            }

            var diagnostic = Diagnostic.Create(Rule, invocationExpr.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
