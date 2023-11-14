// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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

        public void UsingMsBuildAtPath(string instanceMsBuildPath)
        {
            throw new System.NotImplementedException();
        }
    }
}
