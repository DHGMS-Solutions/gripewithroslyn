// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Dhgms.GripeWithRoslyn.Testing.Strings
{
    /// <summary>
    /// Proofs for Unicode ranges.
    /// </summary>
    public static class UnicodeRangeProof
    {
        /// <summary>
        /// Latin range proof.
        /// </summary>
        public const string Latin = "The quick brown fox jumped over the lazy dog";

        /// <summary>
        /// Arabic range proof.
        /// </summary>
        public const string Arabic = "سلام";

        /// <summary>
        /// Latin and Arabic range proof.
        /// </summary>
        public const string LatinAndArabic = "Hello سلام";
    }
}
