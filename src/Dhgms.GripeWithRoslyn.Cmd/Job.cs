﻿// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
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
using Dhgms.GripeWithRoslyn.Cmd.CommandLine;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;

namespace Dhgms.GripeWithRoslyn.Cmd
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
                foreach (var project in solution.Projects)
                {
                    if (project.FilePath == null)
                    {
                        continue;
                    }

                    _logMessageActionsWrapper.StartingAnalysisOfProject(project.FilePath);
                    var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
                    if (compilation == null)
                    {
                        // TODO: warn about failure to get compilation object.
                        _logMessageActionsWrapper.FailedToGetCompilationObjectForProject(project.FilePath);
                        hasIssues = true;
                        continue;
                    }

                    var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
                    var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);
                    hasIssues |= !diagnostics.IsEmpty;
                }
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
                    _logMessageActionsWrapper.MultipleMsBuildInstancesFound(visualStudioInstances.Length);

                    return 3;
            }

            return await DoAnalysis(
                instance,
                commandLineArgModel.SolutionPath).ConfigureAwait(false);
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
                new DoNotUseXUnitInlineDataAttributeAnalyzer());

            var analyzers = analyzersBuilder.ToImmutable();
            return analyzers;
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
