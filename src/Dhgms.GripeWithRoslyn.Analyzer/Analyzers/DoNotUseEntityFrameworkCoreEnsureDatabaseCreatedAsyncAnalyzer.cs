﻿using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class DoNotUseEntityFrameworkCoreEnsureDatabaseCreatedAsyncAnalyzer : BaseInvocationExpressionAnalyzer
    {
        private const string Title = "Do not use Entity Framework Database EnsureCreatedAsync.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "EnsureCreatedAsync should not be used in an Application. There are risks around race conditions, initialization time and complexities with high availability when running multiple nodes. Database releases should be done as part of an independent release process.";

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

        protected override string MethodName => "EnsureCreated";

        protected override string[] ContainingTypes => new []
        {
            "Microsoft.EntityFrameworkCore.Infrastructure",
        };
    }
}
