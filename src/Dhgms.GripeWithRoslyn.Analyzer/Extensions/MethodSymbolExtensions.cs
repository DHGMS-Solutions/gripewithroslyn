// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMethodSymbol"/>.
    /// </summary>
    public static class MethodSymbolExtensions
    {
        /// <summary>
        /// Checks if the method is defined by an overridden method or an interface.
        /// </summary>
        /// <param name="methodSymbol">Method to check.</param>
        /// <returns>Whether the method is defined by an overridden method or an interface.</returns>
        public static bool IsDefinedByOverridenMethodOrInterface(this IMethodSymbol methodSymbol)
        {
            // Check if it overrides something from a base class
            if (methodSymbol.IsOverride)
            {
                return true;
            }

            if (methodSymbol.ExplicitInterfaceImplementations.Length > 0)
            {
                return true;
            }

            // Check if it implements interface methods
            var implementedInterfaces = methodSymbol.ContainingType.AllInterfaces;

            foreach (var implemented in implementedInterfaces)
            {
                foreach (var ifaceMember in implemented.GetMembers())
                {
                    var impl = methodSymbol.ContainingType.FindImplementationForInterfaceMember(ifaceMember);
                    if (SymbolEqualityComparer.Default.Equals(impl, methodSymbol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
