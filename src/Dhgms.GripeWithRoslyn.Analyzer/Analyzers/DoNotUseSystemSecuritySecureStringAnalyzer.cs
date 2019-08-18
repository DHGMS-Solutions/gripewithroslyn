using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    public sealed class DoNotUseSystemSecuritySecureStringAnalyzer : BaseInvocationUsingClassAnalyzer
    {
        internal const string Title = "Do not use System.Security.SecureString.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Design;

        private const string Description =
            "System.Console suggests code that may not be flexible, or carrying out unintended work such as not using a proper logging implementation.";

        public DoNotUseSystemSecuritySecureStringAnalyzer() : base(
            DiagnosticIdsHelper.DoNotUseSystemSecuritySecureString,
            Title,
            MessageFormat,
            Category,
            Description,
            DiagnosticSeverity.Warning)
        {
        }

        protected override string ClassName => "global::System.Security.SecureString";
    }
}
