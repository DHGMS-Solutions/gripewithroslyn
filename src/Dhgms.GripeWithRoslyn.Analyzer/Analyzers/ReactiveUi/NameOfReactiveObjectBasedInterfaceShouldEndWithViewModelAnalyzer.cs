using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.ReactiveUi
{
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class NameOfReactiveObjectBasedInterfaceShouldEndWithViewModelAnalyzer : BaseInterfaceInheritingTypeShouldEndWithSpecificSuffix
    {
    }
}
