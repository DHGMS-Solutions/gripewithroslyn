using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    /// <summary>
    /// Analyzer to ensure System.Security.SecureString is not used.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class DoNotUseSystemSecuritySecureStringAnalyzer : BaseInvocationUsingClassAnalyzer
    {
        internal const string Title = "Do not use System.Security.SecureString.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "System.Console suggests code that may not be flexible, or carrying out unintended work such as not using a proper logging implementation.";

        /// <summary>
        /// Creates an instance of DoNotUseSystemSecuritySecureStringAnalyzer
        /// </summary>
        public DoNotUseSystemSecuritySecureStringAnalyzer() : base(
            DiagnosticIdsHelper.DoNotUseSystemSecuritySecureString,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Error)
        {
        }

        /// <inhertitdoc />
        protected override string ClassName => "global::System.Security.SecureString";
    }
}
