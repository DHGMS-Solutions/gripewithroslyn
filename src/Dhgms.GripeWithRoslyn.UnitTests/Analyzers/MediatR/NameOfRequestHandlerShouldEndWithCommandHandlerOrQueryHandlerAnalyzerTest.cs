﻿// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.MediatR;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using CodeFixVerifier = Dhgms.GripeWithRoslyn.UnitTests.Verifiers.CodeFixVerifier;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzers.MediatR
{
    /// <summary>
    /// Unit Tests for <see cref="NameOfRequestHandlerShouldEndWithCommandHandlerOrQueryHandlerAnalyzer"/>.
    /// </summary>
    public sealed class NameOfRequestHandlerShouldEndWithCommandHandlerOrQueryHandlerAnalyzerTest : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning.
        /// </summary>
        [Fact]
        public void ReturnsWarning()
        {
            const string test = @"
    namespace MediatR
    {
        public interface IRequestHandler<TRequest, TResponse>
        {
        }

        public class Request : IRequest<Response>
        {
        }

        public class Response
        {
        }

        public class Test : IRequestHandler<Request, Response>
        {
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.MediatRRequestHandlerShouldHaveCommandHandlerOrQueryHandlerSuffix,
                Message = NameOfRequestHandlerShouldEndWithCommandHandlerOrQueryHandlerAnalyzer.Title,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 16, 22)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new NameOfRequestHandlerShouldEndWithCommandHandlerOrQueryHandlerAnalyzer();
        }
    }
}
