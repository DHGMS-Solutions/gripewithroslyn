﻿using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using DiagnosticDescriptor = Microsoft.CodeAnalysis.DiagnosticDescriptor;
using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;
using ReportDiagnostic = Microsoft.CodeAnalysis.ReportDiagnostic;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    /// <summary>
    /// Analyzer for checking that Treat Warnings As Errors is Enabled.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public class TreatWarningsAsErrorsAnalyzer : DiagnosticAnalyzer
    {
        private readonly DiagnosticDescriptor _rule;

        /// <summary>
        /// Creates an instance of TreatWarningsAsErrorsAnalyzer
        /// </summary>
        /// <param name="diagnosticId">The Diagnostic Id</param>
        /// <param name="title">The title of the analyzer</param>
        /// <param name="message">The message to display detailing the issue with the analyzer.</param>
        /// <param name="category">The category the analyzer belongs to.</param>
        /// <param name="description">The description of the analyzer</param>
        /// <param name="diagnosticSeverity">The severity assocatiated with breaches of the analyzer</param>
        public TreatWarningsAsErrorsAnalyzer(
            [NotNull] string diagnosticId,
            [NotNull] string title,
            [NotNull] string message,
            [NotNull] string category,
            [NotNull] string description,
            DiagnosticSeverity diagnosticSeverity)
        {
            this._rule = new DiagnosticDescriptor(diagnosticId, title, message, category, diagnosticSeverity,
                isEnabledByDefault: true, description: description);
        }

        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this._rule);

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
            context.RegisterCompilationAction(CompilationAnalysis);
        }

        private void CompilationAnalysis(CompilationAnalysisContext context)
        {
            if (context.Compilation.Options.GeneralDiagnosticOption != ReportDiagnostic.Error)
            {
                var diagnostic = Diagnostic.Create(this._rule, Location.None);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}