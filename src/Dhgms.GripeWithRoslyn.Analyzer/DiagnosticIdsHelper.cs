// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Dhgms.GripeWithRoslyn.Analyzer
{
    internal static class DiagnosticIdsHelper
    {
        internal static string FluentDataAutoMapAnalyzer => "GR0001";

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

        internal static string DoNotUseSystemNetServicePointManager => "GR0021";

        internal static string MediatRRequestHandlerShouldHaveCommandHandlerOrQueryHandlerSuffix => "GR0022";

        internal static string MediatRRequestShouldHaveCommandOrQuerySuffix => "GR0023";

        internal static string MediatRResponseShouldHaveCommandResponseOrQueryResponseSuffix => "GR0024";

        internal static string DoNotUseEnumToString => "GR0025";

        internal static string DoNotUseXUnitInlineDataAttribute => "GR0026";

        internal static string ConstructorShouldAcceptLoggingFrameworkArgument => "GR0027";

        internal static string UseReactiveMarblesObservableEventsInsteadOfObservableFromEventPattern => "GR0028";

        internal static string ReactiveMarblesEventsShouldBeToAVariableAssignment => "GR0029";

        internal static string ClassWithAbstractKeyword => "GR0030";

        internal static string MethodsThatUseReturnYieldShouldHaveNameThatBeginsWithEnumerate => "GR0031";

        internal static string DoNotUseManualEventSubscriptions => "GR0032";

        internal static string DoNotUseObjectAsParameterType => "GR0033";

        internal static string ViewModelClassShouldInheritFromViewModelInterface => "GR0034";

        internal static string UseTypeofInsteadOfMethodBaseGetCurrentMethodDeclaringType => "GR0035";

        internal static string UseTypeofInsteadOfSystemTypeGetType => "GR0036";

        internal static string ApiShouldUseGenericActionResult => "GR0037";

        internal static string DoNotUseObjectAsReturnType => "GR0038";

        internal static string DoNotUseObjectAsPropertyType => "GR0039";

        internal static string DoNotUseObjectAsFieldType => "GR0040";

        internal static string DoNotUseObjectAsLocalVariableType => "GR0041";

        internal static string DoNotUseDynamicAsParameterType => "GR0042";

        internal static string ConstructorShouldAcceptSchedulerArgument => "GR0043";

        internal static string ProjectShouldEnableNullableReferenceTypes => "GR0044";

        internal static string MicrosoftAppCenterShouldNotBeUsed => "GR0045";

        internal static string DoNotUseMethodGroups => "GR0046";

        internal static string DoNotUseEntityFrameworkCoreDbSetUpdate => "GR0047";

        internal static string DoNotUseEntityFrameworkCoreDbSetUpdateRange => "GR0048";

        internal static string DoNotUseTuples => "GR0049";

        internal static string UseFileProviderOverload => "GR0050";

        internal static string PublicMethodsShouldHaveDocumentedCodeExamples => "GR0051";
    }
}
