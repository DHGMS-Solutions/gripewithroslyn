// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.UnitTests.Helpers
{
    /// <summary>
    /// Struct that stores information about a Diagnostic appearing in a source.
    /// </summary>
    public struct DiagnosticResult
    {
        private DiagnosticResultLocation[] _locations;

        /// <summary>
        /// Gets or sets the locations that have been reported for a Diagnostic Result.
        /// </summary>
        public DiagnosticResultLocation[] Locations
        {
            get
            {
                if (_locations == null)
                {
                    _locations = new DiagnosticResultLocation[] { };
                }

                return _locations;
            }

            set
            {
                _locations = value;
            }
        }

        /// <summary>
        /// Gets or sets the severity of the diagnostic result.
        /// </summary>
        public DiagnosticSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the id of the diagnostic result.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the message for the diagnostic result.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the path from the first location.
        /// </summary>
        public string Path
        {
            get
            {
                return Locations.Length > 0 ? Locations[0].Path : string.Empty;
            }
        }

        /// <summary>
        /// Gets the line from the first location.
        /// </summary>
        public int Line
        {
            get
            {
                return Locations.Length > 0 ? Locations[0].Line : -1;
            }
        }

        /// <summary>
        /// Gets the column from the first location.
        /// </summary>
        public int Column
        {
            get
            {
                return Locations.Length > 0 ? Locations[0].Column : -1;
            }
        }
    }
}