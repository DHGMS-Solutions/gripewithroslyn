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
        protected abstract string BaseInterfaceFullName { get; }

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
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeInterfaceDeclarationExpression, SyntaxKind.InterfaceDeclaration);
        }

        private void AnalyzeInterfaceDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated())
            {
                return;
            }

            if (!(context.Node is InterfaceDeclarationSyntax interfaceDeclarationSyntax))
            {
                return;
            }

            var identifier = interfaceDeclarationSyntax.Identifier;
            if (!identifier.Text.EndsWith(this.ClassNameSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var baseList = interfaceDeclarationSyntax.BaseList;

            if (baseList != null
                && baseList.Types.Count > 0)
            {
                foreach (var baseTypeSyntax in baseList.Types)
                {
                    var baseType = baseTypeSyntax.Type;
                    var typeInfo = ModelExtensions.GetTypeInfo(context.SemanticModel, baseType);

                    if (typeInfo.Type == null)
                    {
                        continue;
                    }

                    var typeFullName = typeInfo.Type.GetFullName();

                    if (typeFullName.Equals(this.BaseInterfaceFullName, StringComparison.Ordinal))
                    {
                        return;
                    }
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(this._rule, identifier.GetLocation()));
        }
    }
}