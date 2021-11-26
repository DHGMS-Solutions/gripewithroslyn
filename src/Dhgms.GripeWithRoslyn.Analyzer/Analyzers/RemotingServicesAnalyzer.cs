﻿using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Roslyn Analyzer for detecting the use of methods in the .NET remoting services namespace.
    /// </summary>
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class RemotingServicesAnalyzer : BaseInvocationUsingNamespaceAnalyzer
    {
        private const string Title = ".NET remoting is legacy technology and should not be used. You should be using a newer technology.";

        private const string MessageFormat = Title;

        private const string Category = SupportedCategories.Maintainability;

        private const string Description =
            ".NET remoting is legacy technology and should not be used. You should be using a newer technology.";

        /// <summary>
        /// Creates an instance of RemotingServicesAnalyzer
        /// </summary>
        public RemotingServicesAnalyzer()
            : base(DiagnosticIdsHelper.RemotingServicesAnalyzer,
                  Title,
                  MessageFormat,
                  Category,
                  Description,
                  DiagnosticSeverity.Warning)
        {
        }

        /// <inhertitdoc />
        protected override string Namespace => "System.Runtime.Remoting";
    }
}
