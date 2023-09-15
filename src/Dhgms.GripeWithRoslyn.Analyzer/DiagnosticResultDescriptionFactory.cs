// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Dhgms.GripeWithRoslyn.Analyzer
{
    internal static class DiagnosticResultDescriptionFactory
    {
        internal static string ConstructorShouldAcceptLoggingFrameworkArgument() => $"Constructors should have a final parameter of \nMicrosoft.Extensions.Logging.ILogging<T> or, \na sublass of Whipstaff.Core.ILogMessageActionsWrapper<T> or,\nXUnit.Abstractions.ITestOutputHelper.\n\nThis is to encourage a design that contains sufficient logging.";
    }
}
