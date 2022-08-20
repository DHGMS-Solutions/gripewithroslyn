// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi
{
    /// <summary>
    /// Analyzer to ensure a class inheriting from ReactiveUI.ReactiveObject has the suffix ViewModel.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class NameOfReactiveObjectBasedClassShouldEndWithViewModelAnalyzer : BaseClassInheritingTypeShouldEndWithSpecificSuffix
    {
        internal const string Title = "Classes based on ReactiveUI.ReactiveObject should end with the suffix \"ViewModel\".";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "ViewModels should follow a consistent design of using ReactiveUI's ReactiveObject and an Interface";

        /// <summary>
        /// Initializes a new instance of the <see cref="NameOfReactiveObjectBasedClassShouldEndWithViewModelAnalyzer"/> class.
        /// </summary>
        public NameOfReactiveObjectBasedClassShouldEndWithViewModelAnalyzer()
            : base(
                DiagnosticIdsHelper.ReactiveObjectClassShouldHaveViewModelSuffix,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string[] NameSuffixes => new[]
        {
            "ViewModel"
        };

        /// <inheritdoc />
        protected override string BaseClassFullName => "global::ReactiveUI.ReactiveObject";
    }
}
