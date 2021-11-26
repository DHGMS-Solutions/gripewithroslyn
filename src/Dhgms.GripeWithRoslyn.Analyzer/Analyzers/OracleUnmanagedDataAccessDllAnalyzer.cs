using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Roslyn Analyzer for detecting usage of methods in the old Oracle.DataAccess dll.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class OracleUnmanagedDataAccessDllAnalyzer : BaseInvocationUsingDllAnalyzer
    {
        private const string Title = "Unmanaged Oracle ODP.NET should be replaced with the Oracle Managed DataAccess.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "Oracle ODP.NET should be replaced with Oracle Managed Data Access as it makes ongoing maintenance easier and doesn't require a full install of oracle on a machine.";

        /// <summary>
        /// Creates an instance of OracleUnmanagedDataAccessDllAnalyzer
        /// </summary>
        public OracleUnmanagedDataAccessDllAnalyzer()
            : base(DiagnosticIdsHelper.OracleUnmanagedDllDataAccessDllAnalyzer,
                  Title,
                  MessageFormat,
                  Category,
                  Description,
                  DiagnosticSeverity.Warning)
        {
        }

        /// <inhertitdoc />
        protected override string AssemblyName => "Oracle.DataAccess";
    }
}
