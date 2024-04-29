// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Logging;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.MediatR;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.StructureMap;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.XUnit;
using Dhgms.GripeWithRoslyn.Analyzer.Project;
using Dhgms.GripeWithRoslyn.DotNetTool.CommandLine;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;

namespace Dhgms.GripeWithRoslyn.DotNetTool
{
    /// <summary>
    /// Job to carry out analysis.
    /// </summary>
    public sealed class Job
    {
        private readonly JobLogMessageActionsWrapper _logMessageActionsWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class.
        /// </summary>
        /// <param name="logMessageActionsWrapper">Log Message Actions Wrapper instance.</param>
        public Job(JobLogMessageActionsWrapper logMessageActionsWrapper)
        {
            _logMessageActionsWrapper = logMessageActionsWrapper;
        }

        /// <summary>
        /// Carry out analysis using specified instance of MSBuild.
        /// </summary>
        /// <param name="instance">Instance of MSBuild to use.</param>
        /// <param name="solutionPath">Solution to analyze.</param>
        /// <returns>0 for success, 1 for failure.</returns>
        public async Task<int> DoAnalysis(
            VisualStudioInstance instance,
            FileInfo solutionPath)
        {
            _logMessageActionsWrapper.UsingMsBuildAtPath(instance.MSBuildPath);

            // NOTE: Be sure to register an instance with the MSBuildLocator
            //       before calling MSBuildWorkspace.Create()
            //       otherwise, MSBuildWorkspace won't MEF compose.
            MSBuildLocator.RegisterInstance(instance);

            var solutionFullPath = solutionPath.FullName;
            var hasIssues = false;
            using (var workspace = MSBuildWorkspace.Create())
            {
                // Print message for WorkspaceFailed event to help diagnosing project load failures.
                // TODO: add subscription handler.
                workspace.WorkspaceFailed += (o, e) => _logMessageActionsWrapper.WorkspaceFailed(e);

                _logMessageActionsWrapper.StartingLoadOfSolution(solutionFullPath);

                // Attach progress reporter so we print projects as they are loaded.
                var solution = await workspace.OpenSolutionAsync(solutionFullPath, new ConsoleProgressReporter());
                _logMessageActionsWrapper.FinishedLoadOfSolution(solutionFullPath);

                var analyzers = GetDiagnosticAnalyzers();

                _logMessageActionsWrapper.StartingAnalysisOfProjects();
                var diagnosticCount = new DiagnosticCountModel();
                foreach (var project in solution.Projects)
                {
                    hasIssues |= await AnalyzeProject(
                        project,
                        analyzers,
                        diagnosticCount)
                        .ConfigureAwait(false);
                }

                OutputDiagnosticCounts(diagnosticCount);
            }

            return hasIssues ? 1 : 0;
        }

        /// <summary>
        /// Handle the command line arguments.
        /// </summary>
        /// <param name="commandLineArgModel">Command Line Argument Model.</param>
        /// <returns>0 for success, non-zero for failure.</returns>
        public async Task<int> HandleCommand(CommandLineArgModel commandLineArgModel)
        {
            // Attempt to set the version of MSBuild.
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();

            var count = visualStudioInstances.Length;

            VisualStudioInstance? instance;
            var specificMsBuildInstance = commandLineArgModel.MsBuildInstanceName;

            switch (count)
            {
                case 0:
                    _logMessageActionsWrapper.NoMsBuildInstanceFound();
                    return 1;
                case 1:
                    instance = visualStudioInstances[0];

                    if (!string.IsNullOrWhiteSpace(specificMsBuildInstance))
                    {
                        // check if instance name matches
                        if (!instance.Name.Equals(specificMsBuildInstance, StringComparison.OrdinalIgnoreCase))
                        {
                            _logMessageActionsWrapper.RequestedMsBuildInstanceNotFound(specificMsBuildInstance!);
                            return 2;
                        }
                    }

                    break;
                default:
                    instance = visualStudioInstances.OrderByDescending(x => x.Version).First();
#if TBC
                    _logMessageActionsWrapper.MultipleMsBuildInstancesFound(visualStudioInstances.Length);
                    foreach (var visualStudioInstance in visualStudioInstances)
                    {
                        _logMessageActionsWrapper.FoundMsBuildInstance(visualStudioInstance.Name, visualStudioInstance.MSBuildPath);
                    }
                    return 3;
#endif
                    break;
            }

            return await DoAnalysis(
                instance,
                commandLineArgModel.SolutionPath).ConfigureAwait(false);
        }

        private async Task<bool> AnalyzeProject(Project project, ImmutableArray<DiagnosticAnalyzer> analyzers, DiagnosticCountModel diagnosticCount)
        {
            if (project.FilePath == null)
            {
                return false;
            }

            _logMessageActionsWrapper.StartingAnalysisOfProject(project.FilePath);
            var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
            if (compilation == null)
            {
                // TODO: warn about failure to get compilation object.
                _logMessageActionsWrapper.FailedToGetCompilationObjectForProject(project.FilePath);
                return true;
            }

            var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
            var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);

            OutputDiagnostics(diagnostics, diagnosticCount);

            return !diagnostics.IsEmpty;
        }

        private ImmutableArray<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
        {
            // TODO: build source generator to detect these at build time instead of manual maintenance. got basics of logic in Vetuviem.
            var analyzersBuilder = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();

            analyzersBuilder.AddRange(
                new BooleanTryMethodShouldBeUsedInLogicalNotIfStatementAnalyzer(),
                new ConstructorShouldNotInvokeExternalMethodsAnalyzer(),
                new UseTypeofInsteadOfBaseMethodDeclaringTypeAnalyzer(),
                new UseTypeofInsteadOfTypeGetTypeAnalyzer(),
                new ConstructorShouldAcceptLoggingFrameworkArgumentAnalyzer(),
                new NameOfRequestHandlerShouldEndWithCommandHandlerOrQueryHandlerAnalyzer(),
                new NameOfRequestShouldEndWithCommandOrQueryAnalyzer(),
                new RequestResponseTypeShouldHaveSpecificNameAnalyzer(),
                new NameOfReactiveObjectBasedClassShouldEndWithViewModelAnalyzer(),
                new NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzer(),
                new ViewModelClassShouldInheritFromViewModelInterfaceAnalyzer(),
                new ViewModelClassShouldInheritReactiveObjectAnalyzer(),
                new ViewModelInterfaceShouldInheritReactiveObjectAnalyzer(),
                new DoNotUseEnumToStringAnalyzer(),
                new DoNotUseGdiPlusAnalyzer(),
                new DoNotUseSystemConsoleAnalyzer(),
                new DoNotUseSystemNetServicePointManagerAnalyzer(),
                new DoNotUseSystemSecuritySecureStringAnalyzer(),
                new UseDateTimeUtcNowInsteadofNowAnalyzer(),
                new UseEncodingUnicodeInsteadOfASCIIAnalyzer(),
                new UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer(),
                new StructureMapShouldNotBeUsedAnalyzer(),
                new DoNotUseXUnitInlineDataAttributeAnalyzer(),
                new ProjectShouldEnableNullableReferenceTypesAnalyzer());

            var analyzers = analyzersBuilder.ToImmutable();
            return analyzers;
        }

        private void OutputDiagnosticCounts(DiagnosticCountModel diagnosticCount)
        {
            _logMessageActionsWrapper.DiagnosticCount(diagnosticCount);
        }

        private void OutputDiagnostics(ImmutableArray<Diagnostic> diagnostics, DiagnosticCountModel diagnosticCount)
        {
            foreach (var diagnostic in diagnostics)
            {
                OutputDiagnostic(diagnostic, diagnosticCount);
            }
        }

        private void OutputDiagnostic(Diagnostic diagnostic, DiagnosticCountModel diagnosticCount)
        {
            try
            {
                var message = diagnostic.ToString();
                switch (diagnostic.Severity)
                {
                    case DiagnosticSeverity.Error:
                        diagnosticCount.ErrorCount.Increment();
                        _logMessageActionsWrapper.DiagnosticError(message);
                        break;
                    case DiagnosticSeverity.Hidden:
                        diagnosticCount.HiddenCount.Increment();
                        _logMessageActionsWrapper.DiagnosticHidden(message);
                        break;
                    case DiagnosticSeverity.Info:
                        diagnosticCount.InformationCount.Increment();
                        _logMessageActionsWrapper.DiagnosticInfo(message);
                        break;
                    case DiagnosticSeverity.Warning:
                        diagnosticCount.WarningCount.Increment();
                        _logMessageActionsWrapper.DiagnosticWarning(message);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public void Report(ProjectLoadProgress loadProgress)
            {
                var projectDisplay = Path.GetFileName(loadProgress.FilePath);
                if (loadProgress.TargetFramework != null)
                {
                    projectDisplay += $" ({loadProgress.TargetFramework})";
                }

                Console.WriteLine($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
            }
        }
    }
}
