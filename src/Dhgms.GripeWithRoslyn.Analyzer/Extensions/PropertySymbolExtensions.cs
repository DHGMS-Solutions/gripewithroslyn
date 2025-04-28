// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IPropertySymbol"/>.
    /// </summary>
    public static class PropertySymbolExtensions
    {
        /// <summary>
        /// Checks if the property is defined by an overridden property or an interface.
        /// </summary>
        /// <param name="propertySymbol">Property to check.</param>
        /// <returns>Whether the method is defined by an overridden method or an interface.</returns>
        public static bool IsDefinedByOverridenMethodOrInterface(this IPropertySymbol propertySymbol)
        {
            // Check if it overrides something from a base class
            if (propertySymbol.IsOverride)
            {
                return true;
            }

            if (propertySymbol.ExplicitInterfaceImplementations.Length > 0)
            {
                return true;
            }

            return propertySymbol.ImplementsInterface();
        }
    }
}
