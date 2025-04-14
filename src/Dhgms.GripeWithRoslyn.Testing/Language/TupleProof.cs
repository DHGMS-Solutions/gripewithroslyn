// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Language;

namespace Dhgms.GripeWithRoslyn.Testing.Language
{
    /// <summary>
    /// Analyzer Proofs for <see cref="Tuple"/>.
    /// </summary>
    public static class TupleProof
    {
        /// <summary>
        /// Proof of <see cref="Tuple"/> method invocation to trigger <see cref="DoNotUseTuplesAnalyzer"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// TupleProof.GetTuple();
        /// </code>
        /// </example>
        /// <returns>A <see cref="Tuple"/>.</returns>
        public static (int Number, string Name) GetTuple()
        {
            return (1, "Hello");
        }
    }
}
