using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCracker;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    public sealed class UseTypeofInsteadOfTypeGetTypeAnalyzer : BaseInvocationExpressionAnalyzer
    {
        private const string Title = "Consider usage of typeof(x) instead of System.Type.GetType.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "System.Type.GetType may be a misuse of establishling type information. typeof(x) allows for compile time verification of the type.";

        public UseTypeofInsteadOfTypeGetTypeAnalyzer() : base(
            DiagnosticIdsHelper.UseEncodingUnicodeInsteadOfASCII,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        protected override string MethodName => "GetType";

        protected override string[] ContainingTypes => new [] {"global::System.Type"};
    }
}
