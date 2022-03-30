using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi
{
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzer : BaseInterfaceInheritingTypeShouldEndWithSpecificSuffix
    {
        internal const string Title = "Interfaces based on ReactiveUI.IReactiveObject should end with the suffix \"ViewModel\".";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "ViewModels should follow a consistent design of using ReactiveUI's ReactiveObject and an Interface";

        public NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzer()
            : base(DiagnosticIdsHelper.ReactiveObjectInterfaceShouldHaveViewModelSuffix,
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
        protected override string BaseClassFullName => "global::ReactiveUI.IReactiveObject";
    }
}
