using System.Text.RegularExpressions;

namespace Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions
{
    public static class GeneratedCodeAnalysisExtensions
    {
        public static bool IsOnGeneratedFile(this string filePath) =>
            Regex.IsMatch(filePath, @"(\\service|\\TemporaryGeneratedFile_.*|\\assemblyinfo|\\assemblyattributes|\.(g\.i|g|designer|generated|assemblyattributes))\.(cs|vb)$",
                RegexOptions.IgnoreCase);
    }
}