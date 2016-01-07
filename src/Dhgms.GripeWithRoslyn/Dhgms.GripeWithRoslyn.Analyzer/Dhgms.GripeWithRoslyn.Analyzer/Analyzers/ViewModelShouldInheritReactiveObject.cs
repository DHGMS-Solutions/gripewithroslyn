namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;

    using CodeCracker;

    using JetBrains.Annotations;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    using DiagnosticDescriptor = Microsoft.CodeAnalysis.DiagnosticDescriptor;
    using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;
    using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    /// <summary>
    /// test interface
    /// </summary>
    public interface ITest
    {
        
    }

    /// <summary>
    /// Analyzer for checking that a class that has the ViewModel suffix inherits from ReactiveUI.ReactiveObject
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class ViewModelShouldInheritReactiveObject : BaseClassDeclarationSuffixShouldInheritTypes
    {
        private const string DiagnosticId = "DhgmsGripeWithRoslynAnalyzer";

        private const string Title = "ViewModels should inherit from ReactiveUI's ReactiveObject.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "ViewModels should follow a consistent design of using ReactiveUI's ReactiveObject and an Interface";

        /// <summary>
        /// Creates an instance of ViewModelShouldInheritReactiveObject
        /// </summary>
        public ViewModelShouldInheritReactiveObject()
            : base(DiagnosticIdsHelper.ViewModelShouldInheritReactiveObject,
                  Title,
                  MessageFormat,
                  Category,
                  Description,
                  DiagnosticSeverity.Warning)
        {
        }

        /// <summary>
        /// The suffix of the class to check for.
        /// </summary>
        protected override String ClassNameSuffix => "ViewModel";

        /// <summary>
        /// The containing types the method may belong to.
        /// </summary>
        protected override String[] ContainingTypes => new[] { string.Empty };
    }

    /// <summary>
    /// Base class for checking that a suffixed group of classes inherit from expected types
    /// </summary>
    public abstract class BaseClassDeclarationSuffixShouldInheritTypes : DiagnosticAnalyzer, ITest
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
        protected BaseClassDeclarationSuffixShouldInheritTypes(
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
            context.RegisterSyntaxNodeAction(this.AnalyzeInvocationExpression, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated())
            {
                return;
            }

            var typeDeclarationSyntax = context.Node as TypeDeclarationSyntax;

            if (typeDeclarationSyntax == null)
            {
                return;
            }

            var identifier = typeDeclarationSyntax.Identifier;
            if (!identifier.Text.EndsWith(this.ClassNameSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var baseList = typeDeclarationSyntax.BaseList;

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