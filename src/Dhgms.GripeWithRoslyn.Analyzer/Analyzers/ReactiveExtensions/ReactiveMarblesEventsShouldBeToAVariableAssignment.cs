// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveExtensions
{
    /// <summary>
    /// Check to ensure that a Reactive Marbles event extension method invocation is assigned to a variable.
    /// The reason for suggesting this is you can see the Event() call being made multiple times which is creating
    /// multiple copies of the proxy object. You can have a single proxy then handle each subscription as normal.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ReactiveMarblesEventsShouldBeToAVariableAssignment : AbstractInvocationWithAdditionalCheckAnalyzer
    {
        internal const string Title = "Reactive Marbles events should be assigned to a variable.";
        private const string MessageFormat = Title;
        private const string Category = SupportedCategories.Performance;
        private const string Description = "Reactive Marbles events should be assigned to a variable. To prevent multiple instances of the Event Proxy being created.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveMarblesEventsShouldBeToAVariableAssignment"/> class.
        /// </summary>
        public ReactiveMarblesEventsShouldBeToAVariableAssignment()
            : base(
                DiagnosticIdsHelper.ReactiveMarblesEventsShouldBeToAVariableAssignment,
                Title,
                MessageFormat,
                Category,
                Description,
                Microsoft.CodeAnalysis.DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc/>
        protected override string MethodName => "Event";

        /// <inheritdoc/>
        protected override string[] ContainingTypes => new[]
        {
            "ReactiveMarbles.ObservableEvents",
        };

        /// <inheritdoc/>
        protected override bool GetIfShouldReport(SemanticModel semanticModel, MemberAccessExpressionSyntax memberExpression)
        {
            throw new System.NotImplementedException();
        }
    }
}
