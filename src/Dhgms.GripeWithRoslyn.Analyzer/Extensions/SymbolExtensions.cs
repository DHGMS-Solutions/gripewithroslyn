// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ISymbol"/>.
    /// </summary>
    public static class SymbolExtensions
    {
        /// <summary>
        /// Checks if a symbol implements an interface.
        /// </summary>
        /// <param name="symbol">Symbol to check.</param>
        /// <returns>Whether the symbol implements an interface.</returns>
        public static bool ImplementsInterface(this ISymbol symbol)
        {
            // Check if it implements interface methods
            var implementedInterfaces = symbol.ContainingType.AllInterfaces;

            foreach (var implemented in implementedInterfaces)
            {
                foreach (var ifaceMember in implemented.GetMembers())
                {
                    var impl = symbol.ContainingType.FindImplementationForInterfaceMember(ifaceMember);
                    if (SymbolEqualityComparer.Default.Equals(impl, symbol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
