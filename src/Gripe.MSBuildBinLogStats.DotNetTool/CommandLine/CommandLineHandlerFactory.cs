// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.CommandLine;
using System.IO;
using System.IO.Abstractions;
using Whipstaff.CommandLine;

namespace Gripe.MSBuildBinLogStats.DotNetTool.CommandLine
{
    /// <summary>
    /// Factory for creating the root command and binder.
    /// </summary>
    public sealed class CommandLineHandlerFactory : IRootCommandAndBinderFactory<CommandLineArgModelBinder>
    {
        /// <inheritdoc/>
        public RootCommandAndBinderModel<CommandLineArgModelBinder> GetRootCommandAndBinder(IFileSystem fileSystem)
        {
            ArgumentNullException.ThrowIfNull(fileSystem);

#pragma warning disable CA1861 // Avoid constant arrays as arguments
            var assemblyOption = new Option<FileInfo>(
                    [
                        "--binlog-path",
                        "-bl"
                    ],
                    "Path to the MSBuild bin log to parse.")
                {
                    IsRequired = true
                }.SpecificFileExtensionOnly(
                    fileSystem,
                    ".binlog")
                .ExistingOnly(fileSystem);

#pragma warning restore CA1861 // Avoid constant arrays as arguments

            var rootCommand = new RootCommand("Parses a MSBuild bin log file and produces statistics.")
            {
                assemblyOption
            };

            return new RootCommandAndBinderModel<CommandLineArgModelBinder>(
                rootCommand,
                new CommandLineArgModelBinder(assemblyOption));
        }
    }
}
