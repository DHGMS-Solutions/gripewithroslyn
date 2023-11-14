// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
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
                return await CommandLineArgumentHelpers.GetResultFromRootCommand<CommandLineArgModel, CommandLineArgModelBinder>(
                        args,
                        CommandLineArgumentHelpers.GetRootCommandAndBinder,
                        HandleCommand)
                    .ConfigureAwait(false);
            }
            catch
            {
                return int.MaxValue;
            }
        }
    }
}
