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
            // Attempt to set the version of MSBuild.
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();

            var count = visualStudioInstances.Length;

            // TODO: add command line parsing
            var specificMsBuildInstance = "TEST";

            VisualStudioInstance? instance;
            switch (count)
            {
                case 0:
                    Console.WriteLine($"No MSBuild instance found.");
                    return 1;
                case 1:
                    instance = visualStudioInstances[0];

                    if (!string.IsNullOrWhiteSpace(specificMsBuildInstance))
                    {
                        // check if instance name matches
                        if (!instance.Name.Equals(specificMsBuildInstance, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Requested MSBuild instance \"{specificMsBuildInstance}\" not found.");
                            return 2;
                        }
                    }

                    break;
                default:
                    Console.WriteLine($"Multiple MSBuild instances found. Scenario not yet supported because of behavioural issues with different DiscoveryTypes and Roslyn Analyzers.");

                    return 3;
            }

            return await DoAnalysis(
                instance,
                args).ConfigureAwait(false);
        }

        /// <summary>
        /// Carry out analysis using specified instance of MSBuild.
        /// </summary>
        /// <param name="instance">Instance of MSBuild to use.</param>
        /// <param name="args">Command line arguments.</param>
        /// <returns>0 for success, 1 for failure.</returns>
        public static async Task<int> DoAnalysis(
            VisualStudioInstance instance,
            string[] args)
        {
            Console.WriteLine($"Using MSBuild at '{instance.MSBuildPath}' to load projects.");

            // NOTE: Be sure to register an instance with the MSBuildLocator
            //       before calling MSBuildWorkspace.Create()
            //       otherwise, MSBuildWorkspace won't MEF compose.
            MSBuildLocator.RegisterInstance(instance);

            var hasIssues = false;
            using (var workspace = MSBuildWorkspace.Create())
            {
                // Print message for WorkspaceFailed event to help diagnosing project load failures.
                workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);

                var solutionPath = args[0];
                Console.WriteLine($"Loading solution '{solutionPath}'");

                // Attach progress reporter so we print projects as they are loaded.
                var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
                Console.WriteLine($"Finished loading solution '{solutionPath}'");

                var analyzers = GetDiagnosticAnalyzers();

                foreach (var project in solution.Projects)
                {
                    var compilation = await project.GetCompilationAsync().ConfigureAwait(false);
                    if (compilation == null)
                    {
                        // TODO: warn about failure to get compilation object.
                        hasIssues = true;
                        continue;
                    }

                    var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
                    var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);
                    hasIssues |= !diagnostics.IsEmpty;
                }
            }

            return hasIssues ? 1 : 0;
        }

        private static ImmutableArray<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
        {
            var analyzersBuilder = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();

            analyzersBuilder.AddRange(
                new BooleanTryMethodShouldBeUsedInLogicalNotIfStatementAnalyzer(),
                new ConstructorShouldNotInvokeExternalMethodsAnalyzer(),
                new UseTypeofInsteadOfBaseMethodDeclaringTypeAnalyzer(),
                new UseTypeofInsteadOfTypeGetTypeAnalyzer(),
                new ConstructorShouldAcceptLoggingFrameworkArgumentAnalyzer(),
                new NameOfRequestHandlerShouldEndWithCommandHandlerOrQueryHandlerAnalyzer(),
                new NameOfRequestShouldEndWithCommandOrQueryAnalyzer(),
                new RequestResponseTypeShouldHaveSpecificNameAnalyzer(),
                new NameOfReactiveObjectBasedClassShouldEndWithViewModelAnalyzer(),
                new NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzer(),
                new ViewModelClassShouldInheritFromViewModelInterfaceAnalyzer(),
                new ViewModelClassShouldInheritReactiveObjectAnalyzer(),
                new ViewModelInterfaceShouldInheritReactiveObjectAnalyzer(),
                new DoNotUseEnumToStringAnalyzer(),
                new DoNotUseGdiPlusAnalyzer(),
                new DoNotUseSystemConsoleAnalyzer(),
                new DoNotUseSystemNetServicePointManagerAnalyzer(),
                new DoNotUseSystemSecuritySecureStringAnalyzer(),
                new UseDateTimeUtcNowInsteadofNowAnalyzer(),
                new UseEncodingUnicodeInsteadOfASCIIAnalyzer(),
                new UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer(),
                new StructureMapShouldNotBeUsedAnalyzer(),
                new DoNotUseXUnitInlineDataAttributeAnalyzer());

            var analyzers = analyzersBuilder.ToImmutable();
            return analyzers;
        }

        private static VisualStudioInstance SelectVisualStudioInstance(VisualStudioInstance[] visualStudioInstances)
        {
            Console.WriteLine("Multiple installs of MSBuild detected please select one:");
            for (int i = 0; i < visualStudioInstances.Length; i++)
            {
                Console.WriteLine($"Instance {i + 1}");
                Console.WriteLine($"    Name: {visualStudioInstances[i].Name}");
                Console.WriteLine($"    Version: {visualStudioInstances[i].Version}");
                Console.WriteLine($"    MSBuild Path: {visualStudioInstances[i].MSBuildPath}");
            }

            while (true)
            {
                var userResponse = Console.ReadLine();
                if (int.TryParse(userResponse, out int instanceNumber) &&
                    instanceNumber > 0 &&
                    instanceNumber <= visualStudioInstances.Length)
                {
                    return visualStudioInstances[instanceNumber - 1];
                }

                Console.WriteLine("Input not accepted, try again.");
            }
        }

        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public void Report(ProjectLoadProgress loadProgress)
            {
                var projectDisplay = Path.GetFileName(loadProgress.FilePath);
                if (loadProgress.TargetFramework != null)
                {
                    projectDisplay += $" ({loadProgress.TargetFramework})";
                }

                Console.WriteLine($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
            }
        }
    }
}
