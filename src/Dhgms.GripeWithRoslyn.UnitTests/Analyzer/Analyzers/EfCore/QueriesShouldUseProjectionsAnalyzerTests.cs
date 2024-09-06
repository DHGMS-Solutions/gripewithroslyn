// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.EfCore;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using Xunit;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.EfCore
{
    /// <summary>
    /// Units tests for <see cref="QueriesShouldUseProjectionsAnalyzer"/>.
    /// </summary>
    public sealed class QueriesShouldUseProjectionsAnalyzerTests
    {
        /// <summary>
        /// Test to check the analyzer runs.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task ValidateAnalyzerReturns()
        {
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            MSBuildLocator.RegisterInstance(visualStudioInstances[0]);

            using (var workspace = MSBuildWorkspace.Create())
            {
                var project =
                    await workspace.OpenProjectAsync(
                        "C:\\GitHub\\dhgms-solutions\\gripewithroslyn\\src\\Gripe.Testing\\Gripe.Testing.csproj");

                var analyzer = GetAnalyzer();
                var compilation = await project.GetCompilationAsync();
                var compilationWithAnalyzers = compilation.WithAnalyzers([analyzer]);
                var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
                Assert.NotEmpty(diagnostics);
            }
        }

        private DiagnosticAnalyzer GetAnalyzer()
        {
            return new QueriesShouldUseProjectionsAnalyzer();
        }
    }
}
