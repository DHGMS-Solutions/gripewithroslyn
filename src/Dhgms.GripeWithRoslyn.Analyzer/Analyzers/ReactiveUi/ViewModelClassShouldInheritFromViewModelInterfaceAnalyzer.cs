using System;
using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi
{
    public sealed class ViewModelClassShouldInheritFromViewModelInterfaceAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        internal const string Title = "ViewModel classes should inherit from a ViewModel interface.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "ViewModels should follow a consistent design of using ReactiveUI's ReactiveObject and an Interface";

        private string ClassNameSuffix = "ViewModel";

        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this._rule);

        /// <summary>
        /// Creates an instance of ViewModelShouldInheritReactiveObject
        /// </summary>
        public ViewModelClassShouldInheritFromViewModelInterfaceAnalyzer()
        {
            this._rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.ViewModelClassShouldInheritReactiveObject,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                true,
                Description);
        }

        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclarationExpression, SyntaxKind.ClassDeclaration);
        }

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
            if (!identifier.Text.EndsWith(this.ClassNameSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var baseList = classDeclarationSyntax.BaseList;

            var viewModelInterfaceName = $".I{identifier.Text}";
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

                    if (typeFullName.EndsWith(viewModelInterfaceName, StringComparison.Ordinal))
                    {
                        return;
                    }
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(this._rule, identifier.GetLocation()));
        }
    }
}
