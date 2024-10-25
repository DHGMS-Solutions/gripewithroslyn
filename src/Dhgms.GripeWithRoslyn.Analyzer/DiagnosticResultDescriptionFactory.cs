// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Dhgms.GripeWithRoslyn.Analyzer
{
    internal static class DiagnosticResultDescriptionFactory
    {
        internal static string ConstructorShouldAcceptLoggingFrameworkArgument() => $"Constructors should have a final parameter of \nMicrosoft.Extensions.Logging.ILogging<T> or, \na sublass of Whipstaff.Core.ILogMessageActionsWrapper<T> or,\nXUnit.Abstractions.ITestOutputHelper.\n\nThis is to encourage a design that contains sufficient logging.";

        internal static string TreatWarningsAsErrors() => $"Treat warnings as errors should be enabled on the build. This is to avoid issues that are reported as warnings being missed and piling up technical debt.";

        internal static string DoNotUseObjectAsParameterType() => $"Do not use object as a parameter type. This is to avoid issues with type safety.";

        internal static string DoNotUseObjectAsReturnType() => $"Do not use object as a return type. This is to avoid issues with type safety.";

        internal static string DoNotUseObjectAsPropertyType() => $"Do not use object as a property type. This is to avoid issues with type safety.";

        internal static string DoNotUseObjectAsFieldType() => $"Do not use object as a field type. This is to avoid issues with type safety.";

        internal static string DoNotUseObjectAsLocalVariableType() => $"Do not use object as a local variable type. This is to avoid issues with type safety.";

        internal static string ApiShouldUseGenericActionResult() => $"API methods should return ActionResult<T> to allow easier compile time validating and testing of return results.";

        internal static string DoNotUseDynamicAsParameterType() => $"Do not use dynamic as a parameter type.";

        internal static string ConstructorShouldAcceptSchedulerArgument() => $"ReactiveUI ViewModel Constructors should accept a parameter of \nSystem.Reactive.Concurrency.Scheduler.\n\nThis is to aid with Unit Testing time sensitive logic.";

        internal static string ProjectShouldEnableNullableReferenceTypes() => $"Project should enable Nullable Reference Types. This is to aid with code correctness and to avoid null reference exceptions.";

        internal static string DoNotUseTuples() => $"Do not use Tuples. Consider the use of Record types or Classes. This allows for easier to read and more sustainable code.";
    }
}
