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
    /// <summary>
    /// Base class for checking that a suffixed group of classes inherit from expected types
    /// </summary>
    public abstract class BaseInterfaceDeclarationSuffixShouldInheritTypes : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Creates an instance of BaseInterfaceDeclarationSuffixShouldInheritTypes
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id</param>
        /// <param name="title">The title of the analyzer</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer</param>
        /// <param name="diagnosticSeverity">The severity assocatiated with breaches of the analyzer</param>
        protected BaseInterfaceDeclarationSuffixShouldInheritTypes(
            [NotNull] string diagnosticId,
            [NotNull] string title,
            [NotNull] string message,
            [NotNull] string category,
            [NotNull] string description,
            DiagnosticSeverity diagnosticSeverity)
        {
            this._rule = new DiagnosticDescriptor(diagnosticId, title, message, category, diagnosticSeverity, isEnabledByDefault: true, description: description);
        }

        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this._rule);

        /// <summary>
        /// The suffix of the class to check for.
        /// </summary>
        [NotNull]
        protected abstract string ClassNameSuffix { get; }

        /// <summary>
        /// The containing types the method may belong to.
        /// </summary>
        [NotNull]
        protected abstract string[] ContainingTypes { get; }

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <remarks>
        /// https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        /// </remarks>
        /// <param name="context">
        /// Roslyn context.
        /// </param>
        public sealed override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.AnalyzeClassDeclarationExpression, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeClassDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated())
            {
                return;
            }

            var interfaceDeclarationSyntax = context.Node as InterfaceDeclarationSyntax;

            if (interfaceDeclarationSyntax == null)
            {
                return;
            }

            var identifier = interfaceDeclarationSyntax.Identifier;
            if (!identifier.Text.EndsWith(this.ClassNameSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var baseList = interfaceDeclarationSyntax.BaseList;

            if (baseList != null)
            {
                this.CheckContainingTypeCollection(context, baseList, identifier);
            }
            else
            {
                this.FlagAllContainingTypes(context, identifier);

                context.ReportDiagnostic(Diagnostic.Create(this._rule, identifier.GetLocation()));
            }

        }

        private void FlagAllContainingTypes(SyntaxNodeAnalysisContext context, SyntaxToken identifier)
        {
            foreach (var containingType in this.ContainingTypes)
            {
                context.ReportDiagnostic(Diagnostic.Create(this._rule, identifier.GetLocation()));
            }
        }

        private void CheckContainingTypeCollection(
            SyntaxNodeAnalysisContext context,
            BaseListSyntax baseList,
            SyntaxToken identifier)
        {
            var baseTypes = baseList.Types;

            foreach (var containingType in this.ContainingTypes)
            {
                this.CheckContainingType(context, identifier, containingType, baseTypes);
            }
        }

        private void CheckContainingType(SyntaxNodeAnalysisContext context, SyntaxToken identifier, String containingType, SeparatedSyntaxList<BaseTypeSyntax> baseTypes)
        {
            foreach (var baseTypeSyntax in baseTypes)
            {
                var identifierNameSyntax = baseTypeSyntax.Type as IdentifierNameSyntax;
                if (identifierNameSyntax != null && identifierNameSyntax.Identifier.Text.Equals(containingType, StringComparison.Ordinal))
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(this._rule, identifier.GetLocation()));
        }
    }
}