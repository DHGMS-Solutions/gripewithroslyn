// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime
{
    /// <summary>
    /// Analyzer to ensure the ToString() method is not used on an enum value.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseEnumToStringAnalyzer : AbstractInvocationWithAdditionalCheckAnalyzer
    {
        internal const string Title = "Do not use ToString() on an enum value.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Performance;

        private const string Description =
            "nameof() operator can be used instead of ToString method on an enum value.";

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseEnumToStringAnalyzer"/> class.
        /// </summary>
        public DoNotUseEnumToStringAnalyzer()
            : base(
                DiagnosticIdsHelper.DoNotUseEnumToString,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc/>
        protected override string MethodName => "ToString";

        /// <inheritdoc/>
        protected override string[] ContainingTypes => Array.Empty<string>();

        /// <inheritdoc/>
        protected override bool GetIfShouldReport(
            SemanticModel semanticModel,
            MemberAccessExpressionSyntax memberExpression)
        {
            if (!(memberExpression.Expression is MemberAccessExpressionSyntax parentMemberExpressionExpression))
            {
                return false;
            }

            var typeInfo = ModelExtensions.GetTypeInfo(semanticModel, parentMemberExpressionExpression);
            if (typeInfo.Type == null)
            {
                return false;
            }

            return typeInfo.Type.TypeKind == TypeKind.Enum;
        }
    }
}
