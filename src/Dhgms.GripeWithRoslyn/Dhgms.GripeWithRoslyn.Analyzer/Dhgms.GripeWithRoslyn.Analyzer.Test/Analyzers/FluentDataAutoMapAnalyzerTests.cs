using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dhgms.GripeWithRoslyn.Analyzer.Test.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TestHelper;

    [TestClass]
    public class FluentDataAutoMapAnalyzerTests : CodeFixVerifier
    {
        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public void MethodName()
            {
                Debug.Assert(null);
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "DhgmsGripeWithRoslynAnalyzer",
                Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 15)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        //protected override CodeFixProvider GetCSharpCodeFixProvider()
        //{
        //    return new DhgmsGripeWithRoslynAnalyzerCodeFixProvider();
        //}

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new FluentDataAutoMapAnalyzer();
        }
    }
}
