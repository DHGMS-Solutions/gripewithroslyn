// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Dhgms.GripeWithRoslyn.Analyzer
{
    internal static class DiagnosticIdsHelper
    {
        internal static string FluentDataAutoMapAnalyzer => "GR0000";

        internal static string FluentDataQueryManyWithNullArgumentAnalyzer => "GR0002";

        internal static string OracleUnmanagedDllDataAccessDllAnalyzer => "GR0003";

        internal static string RemotingServicesAnalyzer => "GR0004";

        internal static string ViewModelClassShouldInheritReactiveObject => "GR0005";

        internal static string TreatWarningsAsErrorsShouldBeEnabled => "GR0006";

        internal static string UseEncodingUnicodeInsteadOfAscii => "GR0007";

        internal static string StructureMapShouldNotBeUsed => "GR0008";

        internal static string ReactiveObjectClassShouldHaveViewModelSuffix => "GR0009";

        internal static string ReactiveObjectInterfaceShouldHaveViewModelSuffix => "GR0010";

        internal static string ViewModelInterfaceShouldInheritReactiveObject => "GR0011";

        internal static string ConstructorShouldNotInvokeExternalMethods => "GR0012";

        internal static string DoNotUseGdiPlus => "GR0013";

        internal static string UseDateTimeUtcNowInsteadofNow => "GR0014";

        internal static string DoNotUseSystemConsole => "GR0015";

        internal static string DoNotUseSystemSecuritySecureString => "GR0016";

        internal static string UseSystemTextJsonInsteadOfNewtonsoftJson => "GR0017";

        internal static string TryParseShouldBeUsedInLogicalNotIfStatement => "GR0018";

        internal static string DoNotUseEntityFrameworkCoreDatabaseEnsureCreated => "GR0019";

        internal static string DoNotUseEntityFrameworkCoreDatabaseEnsureCreatedAsync => "GR0020";
    }
}
