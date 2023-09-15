// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Dhgms.GripeWithRoslyn.Analyzer;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Logging;
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
        /// Test to ensure good code doesn't return a warning.
        /// </summary>
        [Fact]
        public void ReturnsNoWarnings()
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

        public class TypeWithCorrectLoggerType
        {
            public TypeWithWrongLoggerType(Microsoft.Extensions.Logging.ILogger<TypeWithCorrectLoggerType> logger)
            {
            }

            public void SomeMethod()
            {
            }
        }

        public class TypeWithLoggerTypeInCorrectPosition
        {
            public TypeWithLoggerTypeInCorrectPosition(string someArg, Microsoft.Extensions.Logging.ILogger<TypeWithLoggerTypeInCorrectPosition> logger)
            {
            }

            public void SomeMethod()
            {
            }
        }

        /// <summary>
        /// This should not be flagged as it is a message action helper.
        /// </summary>
        public sealed class TypeWithCorrectLoggerTypeAndLogMessageActionsInCorrectOrderMessageActions : Whipstaff.Core.Logging.ILogMessageActions<TypeWithCorrectLoggerTypeAndLogMessageActionsInCorrectOrder>
        {
            public TypeWithCorrectLoggerTypeAndLogMessageActionsInCorrectOrderMessageActions()
            {
            }
        }

        public class TypeWithCorrectLoggerTypeAndLogMessageActionsInCorrectOrder
        {
            public TypeWithCorrectLoggerTypeAndLogMessageActionsInCorrectOrder(TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrderMessageActions someArg, Microsoft.Extensions.Logging.ILogger<TypeWithCorrectLoggerTypeAndLogMessageActionsInCorrectOrder> logger)
            {
            }

            public void SomeMethod()
            {
            }
        }

        public sealed class LogMessageActionWrapper : Whipstaff.Core.Logging.AbstractLogMessageActionsWrapper<TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrder, TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrderMessageActions>
        {
            public void SomeLoggingMethod(int someId)
            {
            }
        }

        public class TypeWithLogMessageActionWrapper
        {
            public TypeWithLogMessageActionWrapper(LogMessageActionWrapper logMessageActionsWrapper)
            {
            }

            public void SomeMethod()
            {
            }
        }

        public class TypeWithNoLoggerTypeButNoMethods
        {
            public TypeWithNoLoggerTypeButNoMethods()
            {
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

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
            public TypeWithWrongLoggerType(Microsoft.Extensions.Logging.ILogger<string> logger)
            {
            }
        }

        public class TypeWithWrongLoggerTypeInWrongPosition
        {
            public TypeWithWrongLoggerTypeInWrongPosition(Microsoft.Extensions.Logging.ILogger<string> logger, string someArg)
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
            public TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrder(Microsoft.Extensions.Logging.ILogger<string> logger, TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrderMessageActions someArg)
            {
            }
        }

        public sealed class LogMessageActionWrapper : Whipstaff.Core.Logging.AbstractLogMessageActionsWrapper<TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrder, TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrderMessageActions>
        {
        }

        public class TypeWithWrongLogMessageType
        {
            public TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrder(Microsoft.Extensions.Logging.ILogger<string> logger, TypeWithWrongLoggerTypeAndLogMessageActionsInWrongOrderMessageActions someArg)
            {
            }
        }
    }";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.ConstructorShouldAcceptLoggingFrameworkArgument,
                    Message = DiagnosticResultTitleFactory.ConstructorShouldAcceptLoggingFrameworkArgument(),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 15, 13),
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.ConstructorShouldAcceptLoggingFrameworkArgument,
                    Message = DiagnosticResultTitleFactory.ConstructorShouldAcceptLoggingFrameworkArgument(),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 22, 13),
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.ConstructorShouldAcceptLoggingFrameworkArgument,
                    Message = DiagnosticResultTitleFactory.ConstructorShouldAcceptLoggingFrameworkArgument(),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 29, 13),
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.ConstructorShouldAcceptLoggingFrameworkArgument,
                    Message = DiagnosticResultTitleFactory.ConstructorShouldAcceptLoggingFrameworkArgument(),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 36, 13),
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.ConstructorShouldAcceptLoggingFrameworkArgument,
                    Message = DiagnosticResultTitleFactory.ConstructorShouldAcceptLoggingFrameworkArgument(),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 46, 13),
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.ConstructorShouldAcceptLoggingFrameworkArgument,
                    Message = DiagnosticResultTitleFactory.ConstructorShouldAcceptLoggingFrameworkArgument(),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 53, 13)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdsHelper.ConstructorShouldAcceptLoggingFrameworkArgument,
                    Message = DiagnosticResultTitleFactory.ConstructorShouldAcceptLoggingFrameworkArgument(),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 64, 13)
                        }
                },
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
