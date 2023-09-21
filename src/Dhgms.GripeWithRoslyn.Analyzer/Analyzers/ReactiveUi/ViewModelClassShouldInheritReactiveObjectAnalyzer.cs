// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi
{
    /// <summary>
    /// Analyzer for checking that a class that has the ViewModel suffix inherits from ReactiveUI.ReactiveObject.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class ViewModelClassShouldInheritReactiveObjectAnalyzer : BaseClassDeclarationSuffixShouldInheritTypes
    {
        internal const string Title = "ViewModel classes should inherit from ReactiveUI.ReactiveObject.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "ViewModels should follow a consistent design of using ReactiveUI's ReactiveObject and an Interface";

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelClassShouldInheritReactiveObjectAnalyzer"/> class.
        /// </summary>
        public ViewModelClassShouldInheritReactiveObjectAnalyzer()
            : base(
                DiagnosticIdsHelper.ViewModelClassShouldInheritReactiveObject,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string ClassNameSuffix => "ViewModel";

        /// <inheritdoc />
        protected override string BaseClassFullName => "global::ReactiveUI.ReactiveObject";
    }
}