// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.CommandLine;

namespace Dhgms.GripeWithRoslyn.Cmd.CommandLine
{
    /// <summary>
    /// Model that represents the root command and command line binder logic.
    /// </summary>
    /// <typeparam name="TCommandLineBinder">The type for the command line binding logic.</typeparam>
    public sealed class RootCommandAndBinderModel<TCommandLineBinder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootCommandAndBinderModel{TCommandLineBinder}"/> class.
        /// </summary>
        /// <param name="rootCommand">The root command.</param>
        /// <param name="commandLineBinder">The command line binding logic.</param>
        public RootCommandAndBinderModel(RootCommand rootCommand, TCommandLineBinder commandLineBinder)
        {
            RootCommand = rootCommand;
            CommandLineBinder = commandLineBinder;
        }

        /// <summary>
        /// Gets the root command.
        /// </summary>
        public RootCommand RootCommand { get; }

        /// <summary>
        /// Gets the command line binder.
        /// </summary>
        public TCommandLineBinder CommandLineBinder { get; }
    }
}
