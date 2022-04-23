// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi
{
    using DiagnosticSeverity = DiagnosticSeverity;

    /// <summary>
    /// Analyzer for checking that a class that has the ViewModel suffix inherits from ReactiveUI.ReactiveObject
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class ViewModelInterfaceShouldInheritReactiveObject : BaseInterfaceDeclarationSuffixShouldInheritTypes
    {
        internal const string Title = "ViewModels should inherit from ReactiveUI's ReactiveObject.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "ViewModels should follow a consistent design of using ReactiveUI's ReactiveObject and an Interface";

        /// <summary>
        /// Creates an instance of ViewModelShouldInheritReactiveObject
        /// </summary>
        public ViewModelInterfaceShouldInheritReactiveObject()
            : base(DiagnosticIdsHelper.ViewModelInterfaceShouldInheritReactiveObject,
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
        protected override string BaseInterfaceFullName => "global::ReactiveUI.IReactiveObject";
    }
}