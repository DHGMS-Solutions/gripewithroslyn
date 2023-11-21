// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dhgms.GripeWithRoslyn.Cmd.CommandLine;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dhgms.GripeWithRoslyn.Cmd
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
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var job = new Job(new JobLogMessageActionsWrapper(new NullLogger<Job>(), new JobLogMessageActions()));

                return await CommandLineArgumentHelpers.GetResultFromRootCommand<CommandLineArgModel, CommandLineArgModelBinder>(
                        args,
                        CommandLineArgumentHelpers.GetRootCommandAndBinder,
                        job.HandleCommand)
                    .ConfigureAwait(false);
            }
            catch
            {
                return int.MaxValue;
            }
        }
    }
}
