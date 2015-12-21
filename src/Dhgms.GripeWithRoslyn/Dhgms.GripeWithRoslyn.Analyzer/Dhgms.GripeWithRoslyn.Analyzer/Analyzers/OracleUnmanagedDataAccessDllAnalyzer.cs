using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using CodeCracker;

    using Microsoft.CodeAnalysis;

    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class OracleUnmanagedDataAccessDllAnalyzer : BaseInvocationUsingDllAnalyzer
    {
        public const string DiagnosticId = "DhgmsGripeWithRoslynAnalyzer";

        private const string Title = "Unmanaged Oracle ODP.NET should be replaced with the Oracle Managed DataAccess.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            "Oracle ODP.NET should be replaced with Oracle Managed Data Access as it makes ongoing maintenance easier and doesn't require a full install of oracle on a machine.";

        protected override string AssemblyName => "Oracle.DataAccess";

        public OracleUnmanagedDataAccessDllAnalyzer()
            : base("GR0003",
                  Title,
                  MessageFormat,
                  Category,
                  Description,
                  DiagnosticSeverity.Warning)
        {
        }
    }
}
