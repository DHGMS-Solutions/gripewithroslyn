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
        /// Creates an instance of NameOfReactiveObjectBasedClassShouldEndWithViewModelAnalyzer
        /// </summary>
        public NameOfReactiveObjectBasedClassShouldEndWithViewModelAnalyzer()
            : base(DiagnosticIdsHelper.ReactiveObjectClassShouldHaveViewModelSuffix,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inhertitdoc />
        protected override string NameSuffix => "ViewModel";

        /// <inhertitdoc />
        protected override string BaseClassFullName => "global::ReactiveUI.ReactiveObject";
    }
}
