// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Abstractions;
using Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers
{
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
        /// Initializes a new instance of the <see cref="OracleUnmanagedDataAccessDllAnalyzer"/> class.
        /// </summary>
        public OracleUnmanagedDataAccessDllAnalyzer()
            : base(
                DiagnosticIdsHelper.OracleUnmanagedDllDataAccessDllAnalyzer,
                Title,
                MessageFormat,
                Category,
                Description,
                DiagnosticSeverity.Warning)
        {
        }

        /// <inheritdoc />
        protected override string AssemblyName => "Oracle.DataAccess";
    }
}
