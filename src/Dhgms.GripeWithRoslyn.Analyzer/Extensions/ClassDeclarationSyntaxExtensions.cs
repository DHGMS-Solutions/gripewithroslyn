// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dhgms.GripeWithRoslyn.Analyzer.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ClassDeclarationSyntax"/>.
    /// </summary>
    public static class ClassDeclarationSyntaxExtensions
    {
        /// <summary>
        /// Checks if the <see cref="ClassDeclarationSyntax"/> has implemented any of the specified types.
        /// </summary>
        /// <param name="classDeclarationSyntax">The class declaration node to check.</param>
        /// <param name="baseClasses">The base classes to check for.</param>
        /// <param name="interfaces">The interfaces to check for.</param>
        /// <param name="semanticModel">The semantic model for the code being checked.</param>
        /// <returns>Whether the <see cref="BaseTypeSyntax"/> has implemented any of the specified types.</returns>
        public static bool HasImplementedAnyOfType(
            this ClassDeclarationSyntax classDeclarationSyntax,
            string[] baseClasses,
            string[] interfaces,
            SemanticModel semanticModel)
        {
            var baseList = classDeclarationSyntax.BaseList;
            if (baseList == null)
            {
                return false;
            }

            var baseTypes = baseList.Types;
            if (baseTypes.Count < 1)
            {
                return false;
            }

            foreach (var baseTypeSyntax in baseTypes)
            {
                if (baseTypeSyntax.HasImplementedAnyOfType(
                        baseClasses,
                        interfaces,
                        semanticModel))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
