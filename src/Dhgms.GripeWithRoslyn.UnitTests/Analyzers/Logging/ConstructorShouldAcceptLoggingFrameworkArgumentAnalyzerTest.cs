﻿// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Logging;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.XUnit;
using Dhgms.GripeWithRoslyn.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Dhgms.GripeWithRoslyn.UnitTests.Analyzers.Logging
{
    /// <summary>
    /// Unit tests for the <see cref="ConstructorShouldAcceptLoggingFrameworkArgumentAnalyzer"/> class.
    /// </summary>
    public sealed class ConstructorShouldAcceptLoggingFrameworkArgumentAnalyzerTest : CodeFixVerifier
    {
        /// <summary>
        /// Test to ensure bad code returns a warning.
        /// </summary>
        [Fact]
        public void ReturnsWarning()
        {
            var test = @"
    namespace Microsoft.Extensions.Logging
    {
        public interface ILogger<T>
        {
        }
    }

    namespace ConsoleApplication1
    {
        using XUnit;

        public class TypeWithEmptyCtor
        {
            public TypeWithEmptyCtor()
            {
            }
        }

        public class TypeWithSingleArgument
        {
            public TypeWithSingleArgument(string someArg)
            {
            }
        }

        public class TypeWithWrongLoggerType
        {
            public TypeWithWrongLoggerType(ILogger<string> logger)
            {
            }
        }

        public class TypeWithWrongLoggerTypeInWrongPosition
        {
            public TypeWithWrongLoggerTypeInWrongPosition(ILogger<string> logger, string someArg)
            {
            }
        }

        /// <summary>
        /// This should not be flagged as it is a message action helper.
        /// </summary>
        public sealed class TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrderMessageActions : ILogMessageActions<TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrder>
        {
            public TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrder()
            {
            }
        }

        public class TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrder
        {
            public TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrder(ILogger<string> logger, TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrderMessageActions someArg)
            {
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = DiagnosticIdsHelper.ConstructorShouldAcceptLoggingFrameworkArgument,
                Message = string.Empty,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 16, 14)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        /// <inheritdoc />
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConstructorShouldAcceptLoggingFrameworkArgumentAnalyzer();
        }
    }
}