// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveExtensions
{
    /// <summary>
    /// Analyzer to check for usage of Observable.FromEventPattern.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseReactiveMarblesObservableEventsInsteadOfObservableFromEventPatternAnalyzer : BaseInvocationExpressionAnalyzer
    {
        internal const string Title = "Do not use Observale.FromEventPattern, use source generated Events from ReactiveMarbles.ObservableEvents instead.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "Do not use Observale.FromEventPattern, use source generated Events from ReactiveMarbles.ObservableEvents instead. This simplifies the creation of the observable along with the data type being consumed in a subscription. Overall reducing the burdon on developers.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UseReactiveMarblesObservableEventsInsteadOfObservableFromEventPatternAnalyzer"/> class.
        /// </summary>
        public UseReactiveMarblesObservableEventsInsteadOfObservableFromEventPatternAnalyzer()
            : base(
                DiagnosticIdsHelper.UseReactiveMarblesObservableEventsInsteadOfObservableFromEventPattern,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc/>
        protected override string MethodName => "FromEventPattern";

        /// <inheritdoc />
        protected override string[] ContainingTypes => new[]
        {
            "System.Reactive.Linq.Observable",
        };
    }
}
