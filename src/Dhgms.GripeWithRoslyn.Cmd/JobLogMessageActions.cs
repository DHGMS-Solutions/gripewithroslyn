﻿// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Dhgms.GripeWithRoslyn.Cmd
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
        private readonly Action<ILogger, WorkspaceDiagnosticEventArgs, Exception?> _workspaceFailed;

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
                "Finished load of solution: {Solution}");

            _startingAnalysisOfProject = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(5, nameof(StartingAnalysisOfProject)),
                "Finished load of solution: {Solution}");

            _failedToGetCompilationObjectForProject = LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(6, nameof(FailedToGetCompilationObjectForProject)),
                "Finished load of solution: {Solution}");

            _noMsBuildInstanceFound = LoggerMessage.Define(
                LogLevel.Error,
                new EventId(7, nameof(NoMsBuildInstanceFound)),
                "Finished load of solution: {Solution}");

            _requestedMsBuildInstanceNotFound = LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(8, nameof(RequestedMsBuildInstanceNotFound)),
                "Finished load of solution: {Solution}");

            _multipleMsBuildInstancesFound = LoggerMessage.Define<int>(
                LogLevel.Error,
                new EventId(9, nameof(MultipleMsBuildInstancesFound)),
                "Finished load of solution: {Solution}");

            _workspaceFailed = LoggerMessage.Define<WorkspaceDiagnosticEventArgs>(
                LogLevel.Error,
                new EventId(10, nameof(WorkspaceFailed)),
                "Workspace failed: {Diagnostic}");
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
            _workspaceFailed(logger, workspaceDiagnosticEventArgs, null);
        }
    }
}
