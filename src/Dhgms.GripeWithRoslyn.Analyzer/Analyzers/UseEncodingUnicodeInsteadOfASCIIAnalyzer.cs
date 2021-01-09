using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    public sealed class UseEncodingUnicodeInsteadOfASCIIAnalyzer : BaseInvocationUsingClassAnalyzer
    {
        private const string Title = "Consider usage of Unicode Encoding instead of ASCII.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Reliability;

        private const string Description =
            "ASCII encoding may cause loss of information if Unicode characters are in use. Consider using System.Text.UnicodeEncoding";

        public UseEncodingUnicodeInsteadOfASCIIAnalyzer() : base(
                DiagnosticIdsHelper.UseEncodingUnicodeInsteadOfASCII,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        protected override string ClassName => "global::System.Text.ASCIIEncoding";
    }
}
