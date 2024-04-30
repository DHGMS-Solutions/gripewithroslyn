// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Tracing;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Dhgms.GripeWithRoslyn.DotNetTool
{
    /// <summary>
    /// Log Message Actions for <see cref="Job"/>.
    /// </summary>
    public sealed class JobLogMessageActions
    {
        private readonly Action<ILogger, string, Exception?> _usingMsBuildAtPath;
        private readonly Action<ILogger, string, Exception?> _startingLoadOfSolution;
        private readonly Action<ILogger, string, Exception?> _finishedLoadOfSolution;
        private readonly Action<ILogger, Exception?> _startingAnalysisOfProjects;
        private readonly Action<ILogger, string, Exception?> _startingAnalysisOfProject;
        private readonly Action<ILogger, string, Exception?> _failedToGetCompilationObjectForProject;
        private readonly Action<ILogger, Exception?> _noMsBuildInstanceFound;
        private readonly Action<ILogger, string, Exception?> _requestedMsBuildInstanceNotFound;
        private readonly Action<ILogger, int, Exception?> _multipleMsBuildInstancesFound;
        private readonly Action<ILogger, string, Exception?> _workspaceFailed;
        private readonly Action<ILogger, string, string, Exception?> _foundMsBuildInstance;
        private readonly Action<ILogger, string, Exception?> _diagnosticError;
        private readonly Action<ILogger, string, Exception?> _diagnosticHidden;
        private readonly Action<ILogger, string, Exception?> _diagnosticInfo;
        private readonly Action<ILogger, string, Exception?> _diagnosticWarning;
        private readonly Action<ILogger, int, int, int, int, Exception?> _diagnosticCount;
        private readonly Action<ILogger, string, int, Exception?> _groupedDiagnosticCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobLogMessageActions"/> class.
        /// </summary>
        public JobLogMessageActions()
        {
            _usingMsBuildAtPath = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(1, nameof(UsingMsBuildAtPath)),
                "Using MSBuild at '{InstanceMsBuildPath}' to load projects.");

            _startingLoadOfSolution = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(2, nameof(StartingLoadOfSolution)),
                "Starting load of solution: {Solution}");

            _finishedLoadOfSolution = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(3, nameof(FinishedLoadOfSolution)),
                "Finished load of solution: {Solution}");

            _startingAnalysisOfProjects = LoggerMessage.Define(
                LogLevel.Information,
                new EventId(4, nameof(StartingAnalysisOfProjects)),
                "Starting analysis of projects in solution.");

            _startingAnalysisOfProject = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(5, nameof(StartingAnalysisOfProject)),
                "Starting analysis of project: {Solution}");

            _failedToGetCompilationObjectForProject = LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(6, nameof(FailedToGetCompilationObjectForProject)),
                "Failed to get compilation object for project: {Solution}");

            _noMsBuildInstanceFound = LoggerMessage.Define(
                LogLevel.Error,
                new EventId(7, nameof(NoMsBuildInstanceFound)),
                "No MSBuild Instance Found.");

            _requestedMsBuildInstanceNotFound = LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(8, nameof(RequestedMsBuildInstanceNotFound)),
                "Requested MSBuild instance not found: {InstanceName}.");

            _multipleMsBuildInstancesFound = LoggerMessage.Define<int>(
                LogLevel.Error,
                new EventId(9, nameof(MultipleMsBuildInstancesFound)),
                "Multiple MSBuild Instance found: {Number}");

            _workspaceFailed = LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(10, nameof(WorkspaceFailed)),
                "Workspace failed: {Diagnostic}");

            _foundMsBuildInstance = LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId(10, nameof(WorkspaceFailed)),
                "MSBuild Instance: {Name} - {Location}");

            _diagnosticError = LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(11, nameof(DiagnosticError)),
                "Diagnostic Error: {Message}");

            _diagnosticHidden = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(12, nameof(DiagnosticHidden)),
                "Diagnostic Hidden: {Message}");

            _diagnosticInfo = LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(13, nameof(DiagnosticInfo)),
                "Diagnostic Info: {Message}");

            _diagnosticWarning = LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(14, nameof(DiagnosticWarning)),
                "Diagnostic Warning: {Message}");

            _diagnosticCount = LoggerMessage.Define<int, int, int, int>(
                LogLevel.Information,
                new EventId(15, nameof(DiagnosticCount)),
                "Diagnostic Counts: Hidden - {HiddenCount}, Information - {InfoCount}, Warning - {WarningCount}, Error - {ErrorCount}");

            _groupedDiagnosticCount = LoggerMessage.Define<string, int>(
                LogLevel.Information,
                new EventId(15, nameof(DiagnosticCount)),
                "Diagnostic Id: {Id}, Count- {Count}.");
        }

        /// <summary>
        /// Logging action for recording the path of the MSBuild instance being used.
        /// </summary>
        /// <param name="logger">Logging framework instance.</param>
        /// <param name="instanceMsBuildPath">Path of the MS Build instance.</param>
        internal void UsingMsBuildAtPath(ILogger<Job> logger, string instanceMsBuildPath)
        {
            _usingMsBuildAtPath(logger, instanceMsBuildPath, null);
        }

        /// <summary>
        /// Logging action for when a solution is being loaded.
        /// </summary>
        /// <param name="logger">Logging framework instance.</param>
        /// <param name="solutionFullPath">Path of the solution.</param>
        internal void StartingLoadOfSolution(ILogger<Job> logger, string solutionFullPath)
        {
            _startingLoadOfSolution(logger, solutionFullPath, null);
        }

        /// <summary>
        /// Logging action for when a solution has been loaded.
        /// </summary>
        /// <param name="logger">Logging framework instance.</param>
        /// <param name="solutionFullPath">Path of the solution.</param>
        internal void FinishedLoadOfSolution(ILogger<Job> logger, string solutionFullPath)
        {
            _finishedLoadOfSolution(logger, solutionFullPath, null);
        }

        /// <summary>
        /// Logging action for when analysis of projects is starting.
        /// </summary>
        /// <param name="logger">Logging framework instance.</param>
        internal void StartingAnalysisOfProjects(ILogger<Job> logger)
        {
            _startingAnalysisOfProjects(logger, null);
        }

        /// <summary>
        /// Logging action for when analysis of a project is starting.
        /// </summary>
        /// <param name="logger">Logging framework instance.</param>
        /// <param name="projectFilePath">Path of the project.</param>
        internal void StartingAnalysisOfProject(ILogger<Job> logger, string projectFilePath)
        {
            _startingAnalysisOfProject(logger, projectFilePath, null);
        }

        /// <summary>
        /// Logging action for when a compilation object could not be retrieved for a project.
        /// </summary>
        /// <param name="logger">Logging framework instance.</param>
        /// <param name="projectFilePath">Path of the project.</param>
        internal void FailedToGetCompilationObjectForProject(ILogger<Job> logger, string projectFilePath)
        {
            _failedToGetCompilationObjectForProject(logger, projectFilePath, null);
        }

        /// <summary>
        /// Logging action for when no MSBuild instance could be found.
        /// </summary>
        /// <param name="logger">Logging framework instance.</param>
        internal void NoMsBuildInstanceFound(ILogger<Job> logger)
        {
            _noMsBuildInstanceFound(logger, null);
        }

        /// <summary>
        /// Logging action for when a specific MSBuild instance was requested but not found.
        /// </summary>
        /// <param name="logger">Logging framework instance.</param>
        /// <param name="specificMsBuildInstance">Name of specific MS Build instance.</param>
        internal void RequestedMsBuildInstanceNotFound(ILogger<Job> logger, string specificMsBuildInstance)
        {
            _requestedMsBuildInstanceNotFound(logger, specificMsBuildInstance, null);
        }

        /// <summary>
        /// Logging action for when multiple MSBuild instances were found.
        /// </summary>
        /// <param name="logger">Logging framework instance.</param>
        /// <param name="length">Number of instances.</param>
        internal void MultipleMsBuildInstancesFound(ILogger<Job> logger, int length)
        {
            _multipleMsBuildInstancesFound(logger, length, null);
        }

        internal void WorkspaceFailed(ILogger<Job> logger, WorkspaceDiagnosticEventArgs workspaceDiagnosticEventArgs)
        {
            _workspaceFailed(logger, workspaceDiagnosticEventArgs.Diagnostic.ToString(), null);
        }

        internal void FoundMsBuildInstance(ILogger<Job> logger, string instanceName, string instancePath)
        {
            _foundMsBuildInstance(logger, instanceName, instancePath, null);
        }

        internal void DiagnosticError(ILogger<Job> logger, string message)
        {
            _diagnosticError(logger, message, null);
        }

        internal void DiagnosticHidden(ILogger<Job> logger, string message)
        {
            _diagnosticHidden(logger, message, null);
        }

        internal void DiagnosticInfo(ILogger<Job> logger, string message)
        {
            _diagnosticInfo(logger, message, null);
        }

        internal void DiagnosticWarning(ILogger<Job> logger, string message)
        {
            _diagnosticWarning(logger, message, null);
        }

        internal void DiagnosticCount(ILogger<Job> logger, DiagnosticCountModel diagnosticCount)
        {
            _diagnosticCount(logger, diagnosticCount.HiddenCount.Value, diagnosticCount.InformationCount.Value, diagnosticCount.WarningCount.Value, diagnosticCount.ErrorCount.Value, null);
        }

        internal void GroupedDiagnosticCount(ILogger<Job> logger, string id, int count)
        {
            _groupedDiagnosticCount(logger, id, count, null);
        }
    }
}
