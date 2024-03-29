﻿// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Dhgms.GripeWithRoslyn.Cmd
{
    /// <summary>
    /// Log Message Actions wrapper for <see cref="Job"/>.
    /// </summary>
    public sealed class JobLogMessageActionsWrapper
    {
        private readonly ILogger<Job> _logger;
        private readonly JobLogMessageActions _logMessageActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobLogMessageActionsWrapper"/> class.
        /// </summary>
        /// <param name="logger">Logging framework instance.</param>
        /// <param name="logMessageActions">Log Message actions instance.</param>
        public JobLogMessageActionsWrapper(ILogger<Job> logger, JobLogMessageActions logMessageActions)
        {
            _logger = logger;
            _logMessageActions = logMessageActions;
        }

        /// <summary>
        /// Logging action for recording the path of the MSBuild instance being used.
        /// </summary>
        /// <param name="instanceMsBuildPath">Path of the MS Build instance.</param>
        public void UsingMsBuildAtPath(string instanceMsBuildPath)
        {
            _logMessageActions.UsingMsBuildAtPath(_logger, instanceMsBuildPath);
        }

        /// <summary>
        /// Logging action for when a solution is being loaded.
        /// </summary>
        /// <param name="solutionFullPath">Path of the solution.</param>
        public void StartingLoadOfSolution(string solutionFullPath)
        {
            _logMessageActions.StartingLoadOfSolution(_logger, solutionFullPath);
        }

        /// <summary>
        /// Logging action for when a solution has been loaded.
        /// </summary>
        /// <param name="solutionFullPath">Path of the solution.</param>
        public void FinishedLoadOfSolution(string solutionFullPath)
        {
            _logMessageActions.FinishedLoadOfSolution(_logger, solutionFullPath);
        }

        /// <summary>
        /// Logging action for when analysis of projects is starting.
        /// </summary>
        public void StartingAnalysisOfProjects()
        {
            _logMessageActions.StartingAnalysisOfProjects(_logger);
        }

        /// <summary>
        /// Logging action for when analysis of a project is starting.
        /// </summary>
        /// <param name="projectFilePath">Path of the project.</param>
        public void StartingAnalysisOfProject(string projectFilePath)
        {
            _logMessageActions.StartingAnalysisOfProject(_logger, projectFilePath);
        }

        /// <summary>
        /// Logging action for when a compilation object could not be retrieved for a project.
        /// </summary>
        /// <param name="projectFilePath">Path of the project.</param>
        public void FailedToGetCompilationObjectForProject(string projectFilePath)
        {
            _logMessageActions.FailedToGetCompilationObjectForProject(_logger, projectFilePath);
        }

        /// <summary>
        /// Logging action for when no MSBuild instance could be found.
        /// </summary>
        public void NoMsBuildInstanceFound()
        {
            _logMessageActions.NoMsBuildInstanceFound(_logger);
        }

        /// <summary>
        /// Logging action for when a specific MSBuild instance was requested but not found.
        /// </summary>
        /// <param name="specificMsBuildInstance">Name of specific MS Build instance.</param>
        public void RequestedMsBuildInstanceNotFound(string specificMsBuildInstance)
        {
            _logMessageActions.RequestedMsBuildInstanceNotFound(_logger, specificMsBuildInstance);
        }

        /// <summary>
        /// Logging action for when multiple MSBuild instances were found.
        /// </summary>
        /// <param name="count">Number of instances.</param>
        public void MultipleMsBuildInstancesFound(int count)
        {
            _logMessageActions.MultipleMsBuildInstancesFound(_logger, count);
        }

        internal void WorkspaceFailed(WorkspaceDiagnosticEventArgs e)
        {
            _logMessageActions.WorkspaceFailed(_logger, e);
        }
    }
}
