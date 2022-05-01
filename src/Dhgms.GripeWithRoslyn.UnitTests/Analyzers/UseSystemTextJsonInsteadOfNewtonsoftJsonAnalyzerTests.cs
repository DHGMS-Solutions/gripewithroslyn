// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using CodeFixVerifier = Dhgms.GripeWithRoslyn.UnitTests.Verifiers.CodeFixVerifier;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzers
{
    /// <summary>
    /// Unit Tests for <see cref="UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer"/>.
    /// </summary>
    public sealed class UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzerTests : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning.
        /// </summary>
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace Newtonsoft.Json
    {
        public static class JsonConvert
        {
            public static object DeserializeObject(string value)
            {
                return value;
            }
        }
    }

    namespace ConsoleApplication1
    {
        using System.Text;

        class TypeName
        {
            public void MethodName()
            {
                global::Newtonsoft.Json.JsonConvert.DeserializeObject(""{}"");
            }
        }
    }";

            var a = MethodBase.GetCurrentMethod().DeclaringType;

            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.UseSystemTextJsonInsteadOfNewtonsoftJson,
                Message = UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 21, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new UseSystemTextJsonInsteadOfNewtonsoftJsonAnalyzer();
        }
    }
}
