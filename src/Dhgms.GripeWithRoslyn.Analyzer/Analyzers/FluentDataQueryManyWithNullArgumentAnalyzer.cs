// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Roslyn Analyzer to check for uses of FluentData's AutoMap method
    /// </summary>
    /// <remarks>
    /// Based upon : https://raw.githubusercontent.com/Wintellect/Wintellect.Analyzers/master/Source/Wintellect.Analyzers/Wintellect.Analyzers/Usage/CallAssertMethodsWithMessageParameterAnalyzer.cs
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class FluentDataQueryManyWithNullArgument : BaseInvocationWithArgumentsAnalzyer
    {
        private const string Title = "FluentData QueryMany should not be used with a null value for the mapper.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "QueryMany without a mapper produces potential technical debt where if you are preparing the database schema for new content the old POCO objects won't map due to not having the corresponding property. This risks taking down your platform\\service. Please use a mapper.";

        /// <summary>
        /// Creates an instance of FluentDataQueryManyWithNullArgument
        /// </summary>
        public FluentDataQueryManyWithNullArgument()
            : base(
                  DiagnosticIdsHelper.FluentDataQueryManyWithNullArgumentAnalyzer,
                  Title,
                  MessageFormat,
                  Category,
                  Description,
                  DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string MethodName => "QueryMany";

        /// <inheritdoc />
        protected override string[] ContainingTypes => new[]
                {
                    "FluentData.ISelectBuilder<TEntity>",
                    "FluentData.IStoredProcedureBuilder<T>",
                    "FluentData.IStoredProcedureBuilderDynamic",
                    "FluentData.IDbCommand",
                    "FluentData.IQuery"
                };

        /// <inheritdoc />
        protected override void OnValidateArguments(SyntaxNodeAnalysisContext context, ArgumentListSyntax argumentList)
        {
            if (argumentList?.Arguments.Count != 1)
            {
                return;
            }

            // todo: try and remove the tostring call
            var arg = argumentList.Arguments[0];
            if (arg.Expression.ToString().Equals("null", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(Rule, arg.Expression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
