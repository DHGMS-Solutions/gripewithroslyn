// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Project
{
    /// <summary>
    /// Analyzer for checking that Nullable Reference Types are enabled.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ProjectShouldEnableNullableReferenceTypesAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "ViewModel Constructor should have accept Scheduler as a parameter.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectShouldEnableNullableReferenceTypesAnalyzer"/> class.
        /// </summary>
        public ProjectShouldEnableNullableReferenceTypesAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.ProjectShouldEnableNullableReferenceTypes,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: DiagnosticResultDescriptionFactory.ProjectShouldEnableNullableReferenceTypes());
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            // ReSharper disable once ConvertClosureToMethodGroup
            context.RegisterCompilationAction(compilationAnalysisContext => OnCompilationAnalysis(compilationAnalysisContext));
        }

        private void OnCompilationAnalysis(CompilationAnalysisContext compilationAnalysisContext)
        {
            var compilation = compilationAnalysisContext.Compilation;
            var compilationOptions = compilation.Options;
            if (compilationOptions.NullableContextOptions != NullableContextOptions.Enable)
            {
                compilationAnalysisContext.ReportDiagnostic(Diagnostic.Create(_rule, Location.None));
            }
        }
    }
}
