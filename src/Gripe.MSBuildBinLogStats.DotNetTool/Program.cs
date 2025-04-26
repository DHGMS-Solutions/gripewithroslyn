// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Build.Logging.StructuredLogger;
using Whipstaff.CommandLine.Hosting;

namespace Gripe.MSBuildBinLogStats.DotNetTool
{
    /// <summary>
    /// Program entry point holder.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>0 for success, 1 for failure.</returns>
        public static int Main(string[] args)
        {
            return HostRunner.RunSimpleCliJob<
                CommandLineJob,
                CommandLineArgModel,
                CommandLineArgModelBinder,
                CommandLineHandlerFactory>(
                args,
                (fileSystem, logger) => new CommandLineJob(
                    new CommandLineJobLogMessageActionsWrapper(
                        new CommandLineJobLogMessageActions(),
                        logger),
                    fileSystem),
                new FileSystem());
        }
    }
}
