// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dhgms.GripeWithRoslyn.Analyzer.Extensions
{
    /// <summary>
    /// Extensions for <see cref="MethodDeclarationSyntax"/>.
    /// </summary>
    public static class MethodDeclarationSyntaxExtensions
    {
        /// <summary>
        /// Checks if the method is defined by an overridden method or an interface.
        /// </summary>
        /// <param name="method">Method to check.</param>
        /// <param name="semanticModel">Semantic model for the code.</param>
        /// <returns>Whether the method is defined by an overridden method or an interface.</returns>
        public static bool IsDefinedByOverridenMethodOrInterface(this MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            // it's a method
            var methodSymbol = Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetDeclaredSymbol(semanticModel, method);
            return methodSymbol is not null && methodSymbol.IsDefinedByOverridenMethodOrInterface();
        }
    }
}
