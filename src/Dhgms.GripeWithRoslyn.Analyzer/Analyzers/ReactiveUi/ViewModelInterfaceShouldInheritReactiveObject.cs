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

        /// <inhertitdoc />
        protected override string ClassNameSuffix => "ViewModel";

        /// <inhertitdoc />
        protected override string BaseInterfaceFullName => "global::ReactiveUI.IReactiveObject";
    }
}