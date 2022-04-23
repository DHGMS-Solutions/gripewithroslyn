// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Roslyn Analyzer to check for uses of FluentData's AutoMap method
    /// </summary>
    /// <remarks>
    /// Based upon : https://raw.githubusercontent.com/code-cracker/code-cracker/master/src/CSharp/CodeCracker/Performance/UseStaticRegexIsMatchAnalyzer.cs
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class FluentDataAutoMapAnalyzer : BaseInvocationExpressionAnalyzer
    {
        private const string Title = "FluentData AutoMap should not be used.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "AutoMap produces potential technical debt where if you are preparing the database schema for new content the old POCO objects won't map due to not having the corresponding property. This risks taking down your platform\\service. Please use a mapper.";

        /// <summary>
        /// Creates an instance of FluentDataAutoMapAnalyzer
        /// </summary>
        public FluentDataAutoMapAnalyzer()
            : base(DiagnosticIdsHelper.FluentDataAutoMapAnalyzer,
                  Title,
                  MessageFormat,
                  Category,
                  Description,
                  DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string MethodName => "AutoMap";

        /// <inheritdoc />
        protected override string[] ContainingTypes => new[]
                {
                    "FluentData.IInsertBuilder<T>",
                    "FluentData.IStoredProcedureBuilderDynamic",
                    "FluentData.IStoredProcedureBuilder<T>",
                    "FluentData.IUpdateBuilderDynamic",
                    "FluentData.IUpdateBuilder<T>",
                };
    }
}
