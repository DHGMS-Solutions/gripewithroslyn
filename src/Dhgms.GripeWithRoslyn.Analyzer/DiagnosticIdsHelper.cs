﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dhgms.GripeWithRoslyn.Analyzer
{
    internal static class DiagnosticIdsHelper
    {
        internal static string FluentDataAutoMapAnalyzer => "GR0000";
        internal static string FluentDataQueryManyWithNullArgumentAnalyzer => "GR0002";
        internal static string OracleUnmanagedDllDataAccessDllAnalyzer => "GR0003";
        internal static string RemotingServicesAnalyzer => "GR0004";
        internal static string ViewModelShouldInheritReactiveObject => "GR0005";
        internal static string TreatWarningsAsErrorsShouldBeEnabled => "GR0006";
        internal static string UseEncodingUnicodeInsteadOfASCII => "GR0007";
        internal static string StructureMapShouldNotBeUsed => "GR0008";
        internal static string ReactiveObjectClassShouldHaveViewModelSuffix => "GR0009";
        internal static string ReactiveObjectInterfaceShouldHaveViewModelSuffix => "GR0010";
    }
}
