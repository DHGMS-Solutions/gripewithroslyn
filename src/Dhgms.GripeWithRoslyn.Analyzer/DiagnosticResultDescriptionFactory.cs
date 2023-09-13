// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Dhgms.GripeWithRoslyn.Analyzer
{
    internal static class DiagnosticResultDescriptionFactory
    {
        internal static string ConstructorShouldAcceptLoggingFrameworkArgument() => "Constructors should have a final parameter of Microsoft.Extensions.Logging.ILogging<T> or a sublass of Whipstaff.Core.ILogMessageActionsWrapper<T>. This is to enccourage a design that contains sufficient logging.";
    }
}
