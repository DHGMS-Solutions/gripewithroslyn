using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class UseTypeofInsteadOfTypeGetTypeAnalyzer : BaseInvocationExpressionAnalyzer
    {
        private const string Title = "Consider usage of typeof(x) instead of System.Type.GetType.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "System.Type.GetType may be a misuse of establishling type information. typeof(x) allows for compile time verification of the type.";

        /// <summary>
        /// Creates an instance of UseTypeofInsteadOfTypeGetTypeAnalyzer
        /// </summary>
        public UseTypeofInsteadOfTypeGetTypeAnalyzer() : base(
            DiagnosticIdsHelper.UseEncodingUnicodeInsteadOfASCII,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        /// <inhertitdoc />
        protected override string MethodName => "GetType";

        /// <inhertitdoc />
        protected override string[] ContainingTypes => new [] {"global::System.Type"};
    }
}
