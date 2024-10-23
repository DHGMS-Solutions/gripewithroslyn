// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.EfCore
{
    /// <summary>
    /// Model for expected diagnostic.
    /// </summary>
    public sealed class ExpectedDiagnosticModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedDiagnosticModel"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="severity">Severity of the diagnostic.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="characterPosition">The character position.</param>
        public ExpectedDiagnosticModel(
            string filePath,
            DiagnosticSeverity severity,
            int lineNumber,
            int characterPosition)
        {
            FilePath = filePath;
            Severity = severity;
            LineNumber = lineNumber;
            CharacterPosition = characterPosition;
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Gets the character position.
        /// </summary>
        public int CharacterPosition { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the diagnostic was matched.
        /// </summary>
        public bool Matched { get; set; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        public DiagnosticSeverity Severity { get; }
    }
}
