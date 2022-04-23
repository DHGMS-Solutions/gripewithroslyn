// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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