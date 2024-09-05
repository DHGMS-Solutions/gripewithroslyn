// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Dhgms.GripeWithRoslyn.Analyzer.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ITypeSymbol"/>.
    /// </summary>
    public static class TypeSymbolExtensions
    {
        /// <summary>
        /// Determines if the type is a primitive type.
        /// </summary>
        /// <param name="type">The type symbol to assess.</param>
        /// <returns>Whether the type is a primitive type.</returns>
        public static bool IsPrimitiveType(this ITypeSymbol type)
        {
            return type.SpecialType switch
            {
                SpecialType.System_Boolean or
                    SpecialType.System_Byte or
                    SpecialType.System_SByte or
                    SpecialType.System_Int16 or
                    SpecialType.System_UInt16 or
                    SpecialType.System_Int32 or
                    SpecialType.System_UInt32 or
                    SpecialType.System_Int64 or
                    SpecialType.System_UInt64 or
                    SpecialType.System_Single or
                    SpecialType.System_Double or
                    SpecialType.System_Char => true,
                _ => false
            };
        }
    }
}
