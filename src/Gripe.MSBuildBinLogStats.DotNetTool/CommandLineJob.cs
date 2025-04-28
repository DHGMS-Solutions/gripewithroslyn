// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Gripe.MSBuildBinLogStats.DotNetTool.CommandLine;
using Microsoft.Build.Logging.StructuredLogger;
using Whipstaff.CommandLine;

namespace Gripe.MSBuildBinLogStats.DotNetTool
{
    /// <summary>
    /// Command line job that handles the parsing of MSBuild bin log files.
    /// </summary>
    public sealed class CommandLineJob : AbstractCommandLineHandler<CommandLineArgModel, CommandLineJobLogMessageActionsWrapper>
    {
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineJob"/> class.
        /// </summary>
        /// <param name="commandLineJobLogMessageActionsWrapper">Wrapper for logging framework messages.</param>
        /// <param name="fileSystem">File System abstraction.</param>
        public CommandLineJob(
            CommandLineJobLogMessageActionsWrapper commandLineJobLogMessageActionsWrapper,
            IFileSystem fileSystem)
            : base(commandLineJobLogMessageActionsWrapper)
        {
            ArgumentNullException.ThrowIfNull(fileSystem);

            _fileSystem = fileSystem;
        }

        /// <inheritdoc/>
        protected override Task<int> OnHandleCommand(CommandLineArgModel commandLineArgModel)
        {
            ArgumentNullException.ThrowIfNull(commandLineArgModel);

            return System.Threading.Tasks.Task.Run(() =>
            {
                LogMessageActionsWrapper.StartingHandleCommand();

                var (warningCounts, errorCounts) = GetStats(commandLineArgModel);

                Console.WriteLine("Warnings:");
                ProduceSummary(warningCounts);

                Console.WriteLine();
                Console.WriteLine("Errors:");
                ProduceSummary(errorCounts);

                return 0;
            });
        }

        private static void ProduceSummary(IDictionary<string, int> codesAndCounts)
        {
            if (codesAndCounts.Count == 0)
            {
                Console.WriteLine("  None");
            }
            else
            {
                foreach (var kvp in codesAndCounts.OrderByDescending(kvp => kvp.Value))
                {
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
            }
        }

        private static (IDictionary<string, int> Warnings, IDictionary<string, int> Errors) GetStats(CommandLineArgModel commandLineArgModel)
        {
            var build = BinaryLog.ReadBuild(commandLineArgModel.BinLogPath.FullName);

            var warningCounts = new Dictionary<string, int>();
            var errorCounts = new Dictionary<string, int>();

            Visit(build, warningCounts, errorCounts);

            return (warningCounts, errorCounts);
        }

        private static void Visit(
            TreeNode node,
            IDictionary<string, int> warningCounts,
            IDictionary<string, int> errorCounts)
        {
            if (node is Warning warning && !string.IsNullOrEmpty(warning.Code))
            {
                if (warningCounts.TryGetValue(warning.Code, out var count))
                {
                    warningCounts[warning.Code] = count + 1;
                }
                else
                {
                    warningCounts[warning.Code] = 1;
                }
            }
            else if (node is BuildError error && !string.IsNullOrEmpty(error.Code))
            {
                if (errorCounts.TryGetValue(error.Code, out var count))
                {
                    errorCounts[error.Code] = count + 1;
                }
                else
                {
                    errorCounts[error.Code] = 1;
                }
            }

            foreach (var child in node.Children)
            {
                if (child is TreeNode treeNode)
                {
                    Visit(treeNode, warningCounts, errorCounts);
                }
            }
        }
    }
}
