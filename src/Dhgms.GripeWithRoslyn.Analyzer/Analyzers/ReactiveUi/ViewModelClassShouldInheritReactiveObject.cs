using System;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi
{
    using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;

    /// <summary>
    /// Analyzer for checking that a class that has the ViewModel suffix inherits from ReactiveUI.ReactiveObject
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class ViewModelClassShouldInheritReactiveObject : BaseClassDeclarationSuffixShouldInheritTypes
    {
        private const string Title = "ViewModel classes should inherit from ReactiveUI.ReactiveObject.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "ViewModels should follow a consistent design of using ReactiveUI's ReactiveObject and an Interface";

        /// <summary>
        /// Creates an instance of ViewModelShouldInheritReactiveObject
        /// </summary>
        public ViewModelClassShouldInheritReactiveObject()
            : base(DiagnosticIdsHelper.ViewModelShouldInheritReactiveObject,
                  Title,
                  MessageFormat,
                  Category,
                  Description,
                  DiagnosticSeverity.Warning)
        {
        }

        /// <summary>
        /// The suffix of the class to check for.
        /// </summary>
        protected override String ClassNameSuffix => "ViewModel";

        /// <summary>
        /// The containing types the method may belong to.
        /// </summary>
        protected override String[] ContainingTypes => new[]
        {
            "ReactiveUi.ReactiveObject"
        };
    }
}