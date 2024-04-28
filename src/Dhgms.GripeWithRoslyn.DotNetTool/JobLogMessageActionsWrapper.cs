// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Build.Tasks;
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

        /// <summary>
        /// Logging action for when an MSBuild instance was found.
        /// </summary>
        /// <param name="instanceName">Name of the MSBuild instance.</param>
        /// <param name="instancePath">Path where the instance is located.</param>
        public void FoundMsBuildInstance(string instanceName, string instancePath)
        {
            _logMessageActions.FoundMsBuildInstance(_logger, instanceName, instancePath);
        }

        /// <summary>
        /// Logging action for when a workspace has failed.
        /// </summary>
        /// <param name="e">Workspace Diagnostic Event Args.</param>
        public void WorkspaceFailed(WorkspaceDiagnosticEventArgs e)
        {
            _logMessageActions.WorkspaceFailed(_logger, e);
        }

        /// <summary>
        /// Logging action for a Roslyn diagnostic error report.
        /// </summary>
        /// <param name="message">Message from the diagnostic.</param>
        public void DiagnosticError(string message)
        {
            _logMessageActions.DiagnosticError(_logger, message);
        }

        /// <summary>
        /// Logging action for a Roslyn diagnostic hidden report.
        /// </summary>
        /// <param name="message">Message from the diagnostic.</param>
        public void DiagnosticHidden(string message)
        {
            _logMessageActions.DiagnosticHidden(_logger, message);
        }

        /// <summary>
        /// Logging action for a Roslyn diagnostic information report.
        /// </summary>
        /// <param name="message">Message from the diagnostic.</param>
        public void DiagnosticInfo(string message)
        {
            _logMessageActions.DiagnosticInfo(_logger, message);
        }

        /// <summary>
        /// Logging action for a Roslyn diagnostic warning report.
        /// </summary>
        /// <param name="message">Message from the diagnostic.</param>
        public void DiagnosticWarning(string message)
        {
            _logMessageActions.DiagnosticWarning(_logger, message);
        }

        /// <summary>
        /// Logging action for a Roslyn diagnostic count report.
        /// </summary>
        /// <param name="diagnosticCount">Count model for what diagnostics were reported.</param>
        public void DiagnosticCount(DiagnosticCountModel diagnosticCount)
        {
            _logMessageActions.DiagnosticCount(_logger, diagnosticCount);
        }
    }
}
