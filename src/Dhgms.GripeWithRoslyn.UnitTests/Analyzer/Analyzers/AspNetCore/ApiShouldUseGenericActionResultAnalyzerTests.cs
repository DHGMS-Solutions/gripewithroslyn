// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.AspNetCore;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzer.Analyzers.AspNetCore
{
    /// <summary>
    /// Unit Tests for <see cref="ApiShouldUseGenericActionResultAnalyzer"/>.
    /// </summary>
    public sealed class ApiShouldUseGenericActionResultAnalyzerTests : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning.
        /// </summary>
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace System.System.Web.Http
    {
        public class ApiController
        {
        }
    }

    namespace ConsoleApplication1
    {
        class TypeName : System.System.Web.Http.ApiController
        {
            public System.Web.Http.IHttpActionResult Get()
            {
                return null;
            }

            public Microsoft.AspNetCore.Mvc.ActionResult ActionResultGet()
            {
                return null;
            }

            public Microsoft.AspNetCore.Mvc.ActionResult<int> ActionResultGetWithInt()
            {
                return null;
            }

            public System.Threading.Task<Microsoft.AspNetCore.Mvc.ActionResult<int>> ActionResultGetAsync()
            {
                return null;
            }
        }
    }";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.ApiShouldUseGenericActionResult,
                    Message = ApiShouldUseGenericActionResultAnalyzer.Title,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                    [
                        new DiagnosticResultLocation("Test0.cs", 13, 13)
                    ]
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.ApiShouldUseGenericActionResult,
                    Message = ApiShouldUseGenericActionResultAnalyzer.Title,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                    [
                        new DiagnosticResultLocation("Test0.cs", 18, 13)
                    ]
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.ApiShouldUseGenericActionResult,
                    Message = ApiShouldUseGenericActionResultAnalyzer.Title,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                    [
                        new DiagnosticResultLocation("Test0.cs", 23, 13)
                    ]
                }
            };

            VerifyCSharpDiagnostic(
                test,
                expected);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ApiShouldUseGenericActionResultAnalyzer();
        }
    }
}