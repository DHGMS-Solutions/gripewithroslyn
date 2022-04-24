// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions
{
    /// <summary>
    /// Extensions for working with analyzers.
    /// </summary>
    public static partial class AnalyzerExtensions
    {
        /// <summary>
        /// Copies the trivia from one syntax node to another.
        /// </summary>
        /// <param name="target">Target node to update.</param>
        /// <param name="source">Source node to copy the trivia from.</param>
        /// <returns>Updated node.</returns>
        public static SyntaxNode WithSameTriviaAs(this SyntaxNode target, SyntaxNode source)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return target
                .WithLeadingTrivia(source.GetLeadingTrivia())
                .WithTrailingTrivia(source.GetTrailingTrivia());
        }

        /// <summary>
        /// Checks to see if a display part is one of a number of different kinds.
        /// </summary>
        /// <param name="displayPart">The display part to check.</param>
        /// <param name="kinds">Collection of Display Part Kinds to check for.</param>
        /// <returns>Whether the display part matches any of the acceptable kinds.</returns>
        public static bool IsAnyKind(this SymbolDisplayPart displayPart, params SymbolDisplayPartKind[] kinds)
        {
            return kinds.Any(kind => displayPart.Kind == kind);
        }

        /// <summary>
        /// Finds the first syntax node (including itself) that matches the required type.
        /// </summary>
        /// <typeparam name="T">The desired type.</typeparam>
        /// <param name="node">The node to walk up.</param>
        /// <returns>Matching syntax node.</returns>
        public static T FirstAncestorOrSelfOfType<T>(this SyntaxNode node)
            where T : SyntaxNode
            =>
            (T)node.FirstAncestorOrSelfOfType(typeof(T));

        /// <summary>
        /// Finds the first syntax node (including itself) that matches any of the required types.
        /// </summary>
        /// <param name="node">The node to walk up.</param>
        /// <param name="types">Collection of types to check against.</param>
        /// <returns>Matching syntax node.</returns>
        public static SyntaxNode FirstAncestorOrSelfOfType(
            this SyntaxNode node,
            params Type[] types)
        {
            var currentNode = node;
            while (true)
            {
                if (currentNode == null)
                {
                    break;
                }

                foreach (var type in types)
                {
                    if (currentNode.GetType() == type)
                    {
                        return currentNode;
                    }
                }

                currentNode = currentNode.Parent;
            }

            return null;
        }

        /// <summary>
        /// Finds the first ancestor syntax node that matches the required type.
        /// </summary>
        /// <typeparam name="T">The desired type.</typeparam>
        /// <param name="node">The node to walk up.</param>
        /// <returns>Matching syntax node.</returns>
        public static T FirstAncestorOfType<T>(this SyntaxNode node)
            where T : SyntaxNode
        {
            var currentNode = node;
            while (true)
            {
                var parent = currentNode.Parent;
                if (parent == null)
                {
                    break;
                }

                var tParent = parent as T;
                if (tParent != null)
                {
                    return tParent;
                }

                currentNode = parent;
            }

            return null;
        }

        /// <summary>
        /// Finds the first ancestor syntax node that matches the required type.
        /// </summary>
        /// <param name="node">The node to walk up.</param>
        /// <param name="types">Collection of types to check for.</param>
        /// <returns>Matching syntax node.</returns>
        public static SyntaxNode FirstAncestorOfType(this SyntaxNode node, params Type[] types)
        {
            var currentNode = node;
            while (true)
            {
                var parent = currentNode.Parent;
                if (parent == null)
                {
                    break;
                }

                foreach (var type in types)
                {
                    if (parent.GetType() == type)
                    {
                        return parent;
                    }
                }

                currentNode = parent;
            }

            return null;
        }

        /// <summary>
        /// Gets method symbols for all the type and implemented types.
        /// </summary>
        /// <param name="typeSymbol">The type symbol to retrieve the methods from.</param>
        /// <returns>Collection of method symbols.</returns>
        public static IList<IMethodSymbol> GetAllMethodsIncludingFromInnerTypes(this INamedTypeSymbol typeSymbol)
        {
            var methods = typeSymbol.GetMembers().OfType<IMethodSymbol>().ToList();
            var innerTypes = typeSymbol.GetMembers().OfType<INamedTypeSymbol>();
            foreach (var innerType in innerTypes)
            {
                methods.AddRange(innerType.GetAllMethodsIncludingFromInnerTypes());
            }

            return methods;
        }

        /// <summary>
        /// Gets all base types and the implementing type (i.e. itself).
        /// </summary>
        /// <param name="typeSymbol">The type to check.</param>
        /// <returns>Collection of Named Types.</returns>
        public static IEnumerable<INamedTypeSymbol> AllBaseTypesAndSelf(this INamedTypeSymbol typeSymbol)
        {
            yield return typeSymbol;
            foreach (var b in AllBaseTypes(typeSymbol))
            {
                yield return b;
            }
        }

        /// <summary>
        /// Gets all base types for a type.
        /// </summary>
        /// <param name="typeSymbol">The type to check.</param>
        /// <returns>Collection of Named Types.</returns>
        public static IEnumerable<INamedTypeSymbol> AllBaseTypes(this INamedTypeSymbol typeSymbol)
        {
            while (typeSymbol.BaseType != null)
            {
                yield return typeSymbol.BaseType;
                typeSymbol = typeSymbol.BaseType;
            }
        }

        /// <summary>
        /// Gets the last identifying segment of a type, if it is qualified.
        /// </summary>
        /// <param name="typeName">The type to check.</param>
        /// <returns>Last segment of name.</returns>
        public static string GetLastIdentifierIfQualifiedTypeName(this string typeName)
        {
            var result = typeName;

            var parameterTypeDotIndex = typeName.LastIndexOf('.');
            if (parameterTypeDotIndex > 0)
            {
                result = typeName.Substring(parameterTypeDotIndex + 1);
            }

            return result;
        }

        /// <summary>
        /// Order symbol syntax tokens.
        /// </summary>
        /// <param name="modifiers">Access modifiers to order.</param>
        /// <returns>Ordered modifiers.</returns>
        public static IEnumerable<SyntaxToken> EnsureProtectedBeforeInternal(this IEnumerable<SyntaxToken> modifiers) => modifiers.OrderByDescending(token => token.RawKind);

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

        /// <summary>
        /// Gets all the containing types in a symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>Collection of contained types.</returns>
        public static IEnumerable<INamedTypeSymbol> GetAllContainingTypes(this ISymbol symbol)
        {
            while (symbol.ContainingType != null)
            {
                yield return symbol.ContainingType;
                symbol = symbol.ContainingType;
            }
        }

        /// <summary>
        /// Checks the minimum common accessibility between 2 accessibility declarations.
        /// </summary>
        /// <param name="accessibility">The first accessibility to check.</param>
        /// <param name="otherAccessibility">The other accessibility to compare.</param>
        /// <returns>The minimum Accessibility declaration.</returns>
        public static Accessibility GetMinimumCommonAccessibility(this Accessibility accessibility, Accessibility otherAccessibility)
        {
            if (accessibility == otherAccessibility || otherAccessibility == Accessibility.Private)
            {
                return accessibility;
            }

            if (otherAccessibility == Accessibility.Public)
            {
                return Accessibility.Public;
            }

            switch (accessibility)
            {
                case Accessibility.Private:
                    return otherAccessibility;
                case Accessibility.ProtectedAndInternal:
                case Accessibility.Protected:
                case Accessibility.Internal:
                    return Accessibility.ProtectedAndInternal;
                case Accessibility.Public:
                    return Accessibility.Public;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Checks if the type is a .net primitive type.
        /// </summary>
        /// <param name="typeSymbol">The type symbol to check.</param>
        /// <returns>Whether the type is a .NET primitive type.</returns>
        public static bool IsPrimitive(this ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Byte:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                case SpecialType.System_Char:
                case SpecialType.System_Double:
                case SpecialType.System_Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}