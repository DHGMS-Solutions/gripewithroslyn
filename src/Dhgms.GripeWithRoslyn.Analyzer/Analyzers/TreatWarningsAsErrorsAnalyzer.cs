// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using DiagnosticDescriptor = Microsoft.CodeAnalysis.DiagnosticDescriptor;
using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;
using ReportDiagnostic = Microsoft.CodeAnalysis.ReportDiagnostic;

#if TBC
namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    /// <summary>
    /// Analyzer for checking that Treat Warnings As Errors is Enabled.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class TreatWarningsAsErrorsAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Treat Warnings as Errors should be enabled on the build.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreatWarningsAsErrorsAnalyzer"/> class.
        /// </summary>
        public TreatWarningsAsErrorsAnalyzer()
        {
            _rule = new DiagnosticDescriptor(
                DiagnosticIdsHelper.ConstructorShouldAcceptLoggingFrameworkArgument,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: DiagnosticResultDescriptionFactory.TreatWarningsAsErrors());
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        /// <inheritdoc />
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterCompilationAction(CompilationAnalysis);
        }

        private void CompilationAnalysis(CompilationAnalysisContext context)
        {
            if (context.Compilation.Options.GeneralDiagnosticOption != ReportDiagnostic.Error)
            {
                var diagnostic = Diagnostic.Create(_rule, Location.None);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
#endif