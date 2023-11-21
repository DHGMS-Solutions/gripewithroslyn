// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;

namespace Dhgms.GripeWithRoslyn.Cmd.CommandLine
{
    /// <summary>
    /// Binding logic for the command line arguments.
    /// </summary>
    public sealed class CommandLineArgModelBinder : BinderBase<CommandLineArgModel>
    {
        private readonly Argument<FileInfo> _solutionArgument;
        private readonly Argument<string?> _msBuildInstanceNameArgument;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgModelBinder"/> class.
        /// </summary>
        /// <param name="solutionArgument">Solution argument to parse and bind against.</param>
        /// <param name="msBuildInstanceNameArgument">MSBuild Instance Name argument to parse and bind against.</param>
        public CommandLineArgModelBinder(
            Argument<FileInfo> solutionArgument,
            Argument<string?> msBuildInstanceNameArgument)
        {
            _solutionArgument = solutionArgument;
            _msBuildInstanceNameArgument = msBuildInstanceNameArgument;
        }

        /// <inheritdoc/>
        protected override CommandLineArgModel GetBoundValue(BindingContext bindingContext)
        {
            var solution = bindingContext.ParseResult.GetValueForArgument(_solutionArgument);
            var msBuildInstanceName = bindingContext.ParseResult.GetValueForArgument(_msBuildInstanceNameArgument);
            return new CommandLineArgModel(solution, msBuildInstanceName);
        }
    }
}