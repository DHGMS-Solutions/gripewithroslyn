// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.FileProviders;

namespace Dhgms.GripeWithRoslyn.Testing.Runtime.Extensions
{
    /// <summary>
    /// Analyzer Proofs for <see cref="IFileProvider"/>.
    /// </summary>
    public static class FileProviderProof
    {
        /// <summary>
        /// Proof that the analyzer triggers when a <see cref="IFileProvider"/> is not passed as an argument where a relevant overload exists.
        /// </summary>
        /// <example>
        /// <code>
        /// FileProviderProof.ProofWarningIsRaised();
        /// </code>
        /// </example>
        public static void ProofWarningIsRaised()
        {
            SomeMethod();
        }

        /// <summary>
        /// Proof that the analyzer doesn't trigger when a <see cref="IFileProvider"/> is passed as an argument, even if another valid overload exists.
        /// </summary>
        /// <example>
        /// <code>
        /// FileProviderProof.ProofWarningIsNotRaisedWhereAlreadyUsingFileProviderArg();
        /// </code>
        /// </example>
        public static void ProofWarningIsNotRaisedWhereAlreadyUsingFileProviderArg()
        {
            var fileProvider = new NullFileProvider();
            SomeMethod(fileProvider);
            SomeMethod(fileProvider, 1);
        }

        private static void SomeMethod()
        {
        }

        private static void SomeMethod(IFileProvider fileProvider)
        {
            ArgumentNullException.ThrowIfNull(fileProvider);
        }

        private static void SomeMethod(IFileProvider fileProvider, int i)
        {
            ArgumentNullException.ThrowIfNull(fileProvider);
            ArgumentOutOfRangeException.ThrowIfLessThan(i, 1);
        }
    }
}
