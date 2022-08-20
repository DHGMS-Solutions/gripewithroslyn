// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzers.Runtime
{
    /// <summary>
    /// Unit Tests for <see cref="DoNotUseSystemNetServicePointManagerAnalyzer"/>.
    /// </summary>
    public class DoNotUseSystemNetServicePointManagerAnalyzerTest : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning for a method invocation.
        /// </summary>
        [Fact]
        public void ReturnsWarningForMethodInvocation()
        {
            var test = @"
    namespace System.Net
    {
        public sealed class ServicePoint
        {
        }

        public sealed class System.Net.Security.RemoteCertificateValidationCallback
        {
        }

        public sealed class ServicePointManager
        {
            public static ServicePoint FindServicePoint(Uri uri)
            {
            }

            public static System.Net.Security.RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; }
        }
    }

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public void MethodName()
            {
                var servicePointManager = new System.Net.ServicePointManager();
                servicePointManager.FindServicePoint();
            }
        }
    }";
            var methodInvoke = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.DoNotUseSystemNetServicePointManager,
                Message = DoNotUseSystemNetServicePointManagerAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 29, 17)
                    }
            };

            VerifyCSharpDiagnostic(
                test,
                methodInvoke);
        }

        /// <summary>
        /// Test to ensure bad code returns a warning for a property access.
        /// </summary>
        [Fact]
        public void ReturnsWarningForPropertyAccess()
        {
            var test = @"
    namespace System.Net
    {
        namespace Security
        {
            public sealed class RemoteCertificateValidationCallback
            {
            }
        }

        public sealed class ServicePoint
        {
        }


        public static class ServicePointManager
        {
            public static ServicePoint FindServicePoint()
            {
            }

            public static System.Net.Security.RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; }
        }
    }

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public void MethodName()
            {
                var remoteCertificateValidationCallback = System.Net.ServicePointManager.RemoteCertificateValidationCallback;
            }
        }
    }";
            var methodInvoke = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.DoNotUseSystemNetServicePointManager,
                Message = DoNotUseSystemNetServicePointManagerAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 32, 59)
                    }
            };

            VerifyCSharpDiagnostic(
                test,
                methodInvoke);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DoNotUseSystemNetServicePointManagerAnalyzer();
        }
    }
}