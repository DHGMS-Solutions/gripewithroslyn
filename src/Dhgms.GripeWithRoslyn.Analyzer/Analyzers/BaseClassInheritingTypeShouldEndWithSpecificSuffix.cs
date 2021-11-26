using System;
using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    public abstract class BaseClassInheritingTypeShouldEndWithSpecificSuffix : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Creates an instance of BaseInvocationExpressionAnalyzer
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id</param>
        /// <param name="title">The title of the analyzer</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer</param>
        /// <param name="diagnosticSeverity">The severity assocatiated with breaches of the analyzer</param>
        protected BaseClassInheritingTypeShouldEndWithSpecificSuffix(
            [NotNull] string diagnosticId,
            [NotNull] string title,
            [NotNull] string message,
            [NotNull] string category,
            [NotNull] string description,
            DiagnosticSeverity diagnosticSeverity)
        {
            this._rule = new DiagnosticDescriptor(diagnosticId, title, message, category, diagnosticSeverity, isEnabledByDefault: true, description: description);
        }

        /// <inhertitdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this._rule);

        /// <inhertitdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclarationExpression, SyntaxKind.ClassDeclaration);
        }

        protected abstract string NameSuffix { get; }

        protected abstract string BaseClassFullName { get; }

        private void AnalyzeClassDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated())
            {
                return;
            }

            if (!(context.Node is ClassDeclarationSyntax classDeclarationSyntax))
            {
                return;
            }

            var identifier = classDeclarationSyntax.Identifier;
            if (identifier.Text.EndsWith(this.NameSuffix, StringComparison.OrdinalIgnoreCase))
            {
                // it does end with the desired suffix
                // no point checking to warn if it should or not.
                // that's not the point of this validator.
                return;
            }

            var baseList = classDeclarationSyntax.BaseList;

            if (baseList == null)
            {
                return;
            }

            if (baseList.Types.Count < 1)
            {

                return;
            }

            foreach (var baseTypeSyntax in baseList.Types)
            {
                var baseType = baseTypeSyntax.Type;
                var typeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, baseType);

                if (typeInfo.Type == null)
                {
                    return;
                }

                var typeFullName = typeInfo.Type.GetFullName();

                if (typeFullName.Equals(this.BaseClassFullName, StringComparison.Ordinal))
                {
                    context.ReportDiagnostic(Diagnostic.Create(this._rule, identifier.GetLocation()));
                    return;
                }
            }
        }
    }
}
