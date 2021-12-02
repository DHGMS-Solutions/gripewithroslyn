using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class UseTypeofInsteadOfBaseMethodDeclaringTypeAnalyzer : BaseSimpleMemberAccessOnTypeAnalyzer
    {
        private const string Title = "Consider usage of typeof(x) instead of MethodBase.GetCurrentMethod().DeclaringType.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "MethodBase.GetCurrentMethod().DeclaringType may be a misuse of establishling type information. typeof(x) allows for compile time verification of the type and avoids reflection.";

        /// <summary>
        /// Creates an instance of UseTypeofInsteadOfBaseMethodDeclaringTypeAnalyzer
        /// </summary>
        public UseTypeofInsteadOfBaseMethodDeclaringTypeAnalyzer() : base(
            DiagnosticIdsHelper.UseEncodingUnicodeInsteadOfASCII,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        /// <inhertitdoc />
        protected override string ClassName => "global::System.Reflection.MethodBase";

        /// <inhertitdoc />
        protected override string MemberName => "DeclaringType";
    }
}
