// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="SyntaxNode"/>.
    /// </summary>
    public static class SyntaxNodeExtensions
    {
        /// <summary>
        /// Gets the first ancestor to a node of a specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of Syntax Node to find.</typeparam>
        /// <param name="instance">Current Syntax node to check.</param>
        /// <returns>Matching ancestor node, if any.</returns>
        public static TResult GetAncestor<TResult>(this SyntaxNode instance)
            where TResult : SyntaxNode
        {
            var currentNode = instance.Parent;

            while (currentNode != null)
            {
                if (currentNode is TResult result)
                {
                    return result;
                }

                currentNode = currentNode.Parent;
            }

            return null;
        }
    }
}
