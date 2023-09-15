// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dhgms.GripeWithRoslyn.Analyzer.Extensions
{
    /// <summary>
    /// Extensions for <see cref="BaseTypeSyntax"/>.
    /// </summary>
    public static class BaseTypeSyntaxExtensions
    {
        /// <summary>
        /// Checks if the <see cref="BaseTypeSyntax"/> has implemented any of the specified types.
        /// </summary>
        /// <param name="baseTypeSyntax">The base type syntax node to check.</param>
        /// <param name="baseClasses">The base classes to check for.</param>
        /// <param name="interfaces">The interfaces to check for.</param>
        /// <param name="semanticModel">The semantic model for the code being checked.</param>
        /// <returns>Whether the <see cref="BaseTypeSyntax"/> has implemented any of the specified types.</returns>
        public static bool HasImplementedAnyOfType(
            this BaseTypeSyntax baseTypeSyntax,
            string[] baseClasses,
            string[] interfaces,
            SemanticModel semanticModel)
        {
            var typeSyntax = baseTypeSyntax.Type;
            var baseTypeInfo = semanticModel.GetTypeInfo(typeSyntax);
            var baseTypeSymbol = baseTypeInfo.Type;

            if (baseTypeSymbol == null)
            {
                return false;
            }

            var baseTypeFullName = baseTypeSymbol.GetFullName();

            if (baseClasses.Any(bc => bc.Equals(baseTypeFullName, StringComparison.Ordinal)))
            {
                return true;
            }

            if (interfaces.Any(i => i.Equals(baseTypeFullName, StringComparison.Ordinal)))
            {
                return true;
            }

            if (baseTypeSymbol.AllInterfaces.Any(symbol =>
                {
                    var fn = symbol.GetFullName();
                    return interfaces.Any(i => i.Equals(fn, StringComparison.Ordinal));
                }))
            {
                return true;
            }

            return false;
        }
    }
}
