// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer
{
    /// <summary>
    /// Extensions Roslyn Symbols.
    /// </summary>
    public static class SymbolExtensions
    {
        /// <summary>
        /// Gets the full name of a symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <param name="addGlobal">Whether to add the global prefix.</param>
        /// <returns>Full name for the type.</returns>
        public static string GetFullName(this ISymbol symbol, bool addGlobal = true)
        {
            if (symbol.Kind == SymbolKind.TypeParameter)
            {
                return symbol.ToString();
            }

            if (symbol.Kind == SymbolKind.ArrayType)
            {
                var arrayTypeSymbol = (IArrayTypeSymbol)symbol;
                return arrayTypeSymbol.ElementType.ToString();
            }

            var fullName = symbol.Name;
            var containingSymbol = symbol.ContainingSymbol;
            while (!(containingSymbol is INamespaceSymbol))
            {
                fullName = $"{containingSymbol.Name}.{fullName}";
                containingSymbol = containingSymbol.ContainingSymbol;
            }

            if (!((INamespaceSymbol)containingSymbol).IsGlobalNamespace)
            {
                fullName = $"{containingSymbol.ToString()}.{fullName}";
            }

            if (addGlobal)
            {
                fullName = $"global::{fullName}";
            }

            return fullName;
        }
    }
}
