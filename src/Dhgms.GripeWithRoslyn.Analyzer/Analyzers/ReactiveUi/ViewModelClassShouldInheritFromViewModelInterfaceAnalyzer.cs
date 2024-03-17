// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi
{
    /// <summary>
    /// Analyzer for checking that a view model class implements a viewmodel interface.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class ViewModelClassShouldInheritFromViewModelInterfaceAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "ViewModel classes should inherit from a ViewModel interface.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "ViewModels should follow a consistent design of using ReactiveUI's ReactiveObject and an Interface";

        private const string ClassNameSuffix = "ViewModel";

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelClassShouldInheritFromViewModelInterfaceAnalyzer"/> class.
        /// </summary>
        public ViewModelClassShouldInheritFromViewModelInterfaceAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.ViewModelClassShouldInheritFromViewModelInterface,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                true,
                Description);
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclarationExpression, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeClassDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ClassDeclarationSyntax classDeclarationSyntax))
            {
                return;
            }

            var identifier = classDeclarationSyntax.Identifier;
            if (!identifier.Text.EndsWith(ClassNameSuffix, StringComparison.OrdinalIgnoreCase))
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

            context.ReportDiagnostic(Diagnostic.Create(_rule, identifier.GetLocation()));
        }
    }
}
