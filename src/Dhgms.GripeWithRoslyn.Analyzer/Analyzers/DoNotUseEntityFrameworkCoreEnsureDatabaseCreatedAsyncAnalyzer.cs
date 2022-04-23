// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    /// <summary>
    /// Analyzer to ensure the EF Core EnsureCreatedAsync method is not used in an Application.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class DoNotUseEntityFrameworkCoreEnsureDatabaseCreatedAsyncAnalyzer : BaseInvocationExpressionAnalyzer
    {
        private const string Title = "Do not use Entity Framework Database EnsureCreatedAsync.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "EnsureCreatedAsync should not be used in an Application. There are risks around race conditions, initialization time and complexities with high availability when running multiple nodes. Database releases should be done as part of an independent release process.";

        /// <summary>
        /// Creates an instance of DoNotUseEntityFrameworkCoreEnsureDatabaseCreatedAsyncAnalyzer.
        /// </summary>
        public DoNotUseEntityFrameworkCoreEnsureDatabaseCreatedAsyncAnalyzer()
            : base(
                DiagnosticIdsHelper.DoNotUseEntityFrameworkCoreDatabaseEnsureCreatedAsync,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string MethodName => "EnsureCreated";

        /// <inheritdoc />
        protected override string[] ContainingTypes => new[]
        {
            "Microsoft.EntityFrameworkCore.Infrastructure",
        };
    }
}
