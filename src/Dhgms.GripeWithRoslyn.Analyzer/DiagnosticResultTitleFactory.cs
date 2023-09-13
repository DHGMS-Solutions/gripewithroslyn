// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Dhgms.GripeWithRoslyn.Analyzer
{
    internal static class DiagnosticResultTitleFactory
    {
        internal static string ConstructorShouldAcceptLoggingFrameworkArgument() => "Constructor should have a logging framework instance as the final parameter.";
    }
}
