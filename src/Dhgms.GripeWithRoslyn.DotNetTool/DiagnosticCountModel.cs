// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Whipstaff.Runtime.Counters;

namespace Dhgms.GripeWithRoslyn.DotNetTool
{
    /// <summary>
    /// Model for diagnostic counts reported at the end of the run.
    /// </summary>
    public sealed class DiagnosticCountModel
    {
        /// <summary>
        /// Gets the IncrementCounter for Number of errors reported.
        /// </summary>
        public IncrementOnlyInt ErrorCount { get; } = new();

        /// <summary>
        /// Gets the IncrementCounter for Number of hidden diagnostics reported.
        /// </summary>
        public IncrementOnlyInt HiddenCount { get; } = new();

        /// <summary>
        /// Gets the IncrementCounter for Number of information diagnostics reported.
        /// </summary>
        public IncrementOnlyInt InformationCount { get; } = new();

        /// <summary>
        /// Gets the IncrementCounter for Number of warnings reported.
        /// </summary>
        public IncrementOnlyInt WarningCount { get; } = new();
    }
}
