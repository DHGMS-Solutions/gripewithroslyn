// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime
{
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
        /// Initializes a new instance of the <see cref="RemotingServicesAnalyzer"/> class.
        /// </summary>
        public RemotingServicesAnalyzer()
            : base(
                DiagnosticIdsHelper.RemotingServicesAnalyzer,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string Namespace => "System.Runtime.Remoting";
    }
}
