// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;

namespace Dhgms.GripeWithRoslyn.DotNetTool.CommandLine
{
    /// <summary>
    /// Model that represents the command line arguments.
    /// </summary>
    public sealed class CommandLineArgModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgModel"/> class.
        /// </summary>
        /// <param name="solutionPath">Solution to analyze.</param>
        /// <param name="msBuildInstanceName">Name of the MS Build instance to use, if any.</param>
        public CommandLineArgModel(FileInfo solutionPath, string? msBuildInstanceName)
        {
            SolutionPath = solutionPath;
            MsBuildInstanceName = msBuildInstanceName;
        }

        /// <summary>
        /// Gets the solution to analyze.
        /// </summary>
        public FileInfo SolutionPath { get; }

        /// <summary>
        /// Gets the name of the MS Build instance to use, if any.
        /// </summary>
        public string? MsBuildInstanceName { get; }
    }
}
