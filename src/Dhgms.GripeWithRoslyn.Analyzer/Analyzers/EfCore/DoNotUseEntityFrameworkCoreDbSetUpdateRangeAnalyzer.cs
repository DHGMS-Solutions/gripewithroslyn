// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.EfCore
{
    /// <summary>
    /// Analyzer to ensure the EF Core DbSet method is not used in an Application.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseEntityFrameworkCoreDbSetUpdateRangeAnalyzer : BaseInvocationExpressionAnalyzer
    {
        private const string Title = "Do not use Entity Framework DbSet<T>.UpdateRange.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "UpdateRange should not be used in an Application. It invalidates the entire poco object. Just select the desired entities, update the relevant properties and call SaveChangesAsync.";

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseEntityFrameworkCoreDbSetUpdateRangeAnalyzer"/> class.
        /// </summary>
        public DoNotUseEntityFrameworkCoreDbSetUpdateRangeAnalyzer()
            : base(
                DiagnosticIdsHelper.DoNotUseEntityFrameworkCoreDbSetUpdateRange,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string MethodName => "UpdateRange";

        /// <inheritdoc />
        protected override string[] ContainingTypes => new[]
        {
            "Microsoft.EntityFrameworkCore.DbSet",
        };
    }
}