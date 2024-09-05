// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Data;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Dhgms.GripeWithRoslyn.Analyzer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language
{
    /// <summary>
    /// Analyzer to suggest using value objects instead of primitive types in method declarations.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseValueObjectsInsteadOfPrimitiveTypesAnalyzer : DiagnosticAnalyzer
    {
        private const string Title = "Consider using ValueObjects instead of Primitive Types.";

        private const string MessageFormat = "Method '{0}' has a parameter '{1}' of primitive type '{2}. Consider using a ValueObject wrapper.'";

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="UseValueObjectsInsteadOfPrimitiveTypesAnalyzer"/> class.
        /// </summary>
        public UseValueObjectsInsteadOfPrimitiveTypesAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.UseValueObjectsInsteadOfPrimitiveTypes,
                Title,
                MessageFormat,
                SupportedCategories.Maintainability,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Title);
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            foreach (var parameter in methodDeclaration.ParameterList.Parameters)
            {
                if (parameter.Type == null)
                {
                    continue;
                }

                var type = ModelExtensions.GetTypeInfo(context.SemanticModel, parameter.Type).Type;
                if (type == null || !type.IsPrimitiveType())
                {
                    continue;
                }

                // Include the parameter name in the diagnostic message
                var parameterName = parameter.Identifier.Text;
                var diagnostic = Diagnostic.Create(_rule, parameter.GetLocation(), methodDeclaration.Identifier.Text, parameterName, type.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
