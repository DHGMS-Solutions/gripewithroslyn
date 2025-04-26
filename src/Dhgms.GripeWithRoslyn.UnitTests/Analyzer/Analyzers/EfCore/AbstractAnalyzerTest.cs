// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using Xunit;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.EfCore
{
    /// <summary>
    /// Base class for testing analyzers.
    /// </summary>
    /// <typeparam name="TAnalyzer">The type for the analyzer to test.</typeparam>
    public abstract class AbstractAnalyzerTest<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        /// <summary>
        /// Test to ensure anaylzer returns diagnostic results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ReturnsDiagnosticResults()
        {
            var analyzer = new TAnalyzer();
            var immutableArrayBuilder = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();
            immutableArrayBuilder.Add(analyzer);
            var analyzers = immutableArrayBuilder.ToImmutableArray();

            var expectedDiagnostics = GetExpectedDiagnosticLines();
            if (expectedDiagnostics.Length < 1)
            {
                throw new InvalidOperationException("No diagnostics returned from GetExpectedDiagnostics. Won't be a valid test.");
            }

            using (var workspace = MSBuildWorkspace.Create())
            {
                // MSBuildProjectDirectory
                var project =
                    await workspace.OpenProjectAsync(
                        "../../../../Dhgms.GripeWithRoslyn.Testing/Dhgms.GripeWithRoslyn.Testing.csproj");
                var compilation = await project.GetCompilationAsync();
                if (compilation == null)
                {
                    // TODO: warn about failure to get compilation object.
                    throw new InvalidOperationException("Failed to get compilation object");
                }

                var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
                var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

                Assert.NotEmpty(diagnostics);

                var issues = new List<string>();
                var expectedDiagnosticId = GetExpectedDiagnosticId();
                for (var i = 0; i < diagnostics.Length; i++)
                {
                    var diagnostic = diagnostics[i];
                    AnalyseDiagnostic(diagnostic, i, expectedDiagnostics, issues, expectedDiagnosticId);
                }

                foreach (var expectedDiagnostic in expectedDiagnostics.Where(ed => !ed.Matched))
                {
                    issues.Add($"Expected diagnostic {expectedDiagnostic.FilePath} {expectedDiagnostic.Severity} Line:{expectedDiagnostic.LineNumber} Pos:{expectedDiagnostic.CharacterPosition} wasn't found in reported diagnostics.");
                }

                if (issues.Count > 0)
                {
                    var message = new StringBuilder();
                    message.AppendLine("Issues found:");
                    foreach (var issue in issues)
                    {
                        message.AppendLine(issue);
                    }

                    Assert.Fail(message.ToString());
                }
            }
        }

        /// <summary>
        /// Gets the expected diagnostic id.
        /// </summary>
        /// <returns>Expected diagnostic id.</returns>
        protected abstract string GetExpectedDiagnosticId();

        /// <summary>
        /// Gets the expected lines that will report a diagnostic.
        /// </summary>
        /// <returns>Collection of line numbers.</returns>
        protected abstract ExpectedDiagnosticModel[] GetExpectedDiagnosticLines();

        private static void AnalyseDiagnostic(
            Diagnostic diagnostic,
            int index,
            ExpectedDiagnosticModel[] expectedDiagnostics,
            List<string> issues,
            string expectedDiagnosticId)
        {
            var actualSpan = diagnostic.Location.GetLineSpan();

            var actualLinePosition = actualSpan.StartLinePosition;
            var actualLine = actualLinePosition.Line;
            var actualCharacterPosition = actualLinePosition.Character;

            var cantMatch = false;
            if (actualLine < 1)
            {
                issues.Add($"Diagnostic {index} doesn't contain a line.");
                cantMatch = true;
            }

            if (actualLinePosition.Character < 1)
            {
                issues.Add($"Diagnostic {index} doesn't contain a character position.");
                cantMatch = true;
            }

            if (cantMatch)
            {
                issues.Add($"Diagnostic {index} can't be processed to be matched.");
                return;
            }

            foreach (var expectedDiagnostic in expectedDiagnostics.Where(ed => !ed.Matched))
            {
                if (MatchesExpectedDiagnostic(
                        actualSpan,
                        expectedDiagnostic,
                        actualLine,
                        actualCharacterPosition,
                        diagnostic,
                        expectedDiagnosticId))
                {
                    expectedDiagnostic.Matched = true;
                    return;
                }
            }

            issues.Add($"Diagnostic {actualSpan.Path} {diagnostic.Severity} Line:{actualLine} Pos:{actualCharacterPosition} wasn't found in expected collection.");
        }

        private static bool MatchesExpectedDiagnostic(
            FileLinePositionSpan actualSpan,
            ExpectedDiagnosticModel expectedDiagnostic,
            int actualLine,
            int actualCharacterPosition,
            Diagnostic diagnostic,
            string expectedDiagnosticId)
        {
            return actualSpan.Path.EndsWith(expectedDiagnostic.FilePath)
                   && expectedDiagnostic.Severity == diagnostic.Severity
                   && expectedDiagnosticId == diagnostic.Id
                   && expectedDiagnostic.LineNumber == actualLine
                   && expectedDiagnostic.CharacterPosition == actualCharacterPosition;
        }
    }
}
