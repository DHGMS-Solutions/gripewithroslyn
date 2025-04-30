// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace Dhgms.GripeWithRoslyn.Testing.Language
{
    /// <summary>
    /// Analyzer Proofs for constructor external method invocation.
    /// </summary>
    public sealed class ConstructorExternalMethodInvocationProof
    {
        private readonly Lazy<int> _lazyLambaTestShouldNotRaise;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorExternalMethodInvocationProof"/> class.
        /// </summary>
        /// <param name="name">A test name.</param>
        /// <example>
        /// <code>
        /// var proof = new ConstructorExternalMethodInvocationProof("bob");
        /// </code>
        /// </example>
        public ConstructorExternalMethodInvocationProof(string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            // This is a test to ensure the analyzer doesn't raise on lambdas.
            _lazyLambaTestShouldNotRaise = new Lazy<int>(() => 1);

            // This is a test to ensure the analyzer DOES raise on external methods.
            _ = Environment.GetCommandLineArgs();

            // This is a test to ensure the analyzer doesn't raise on private member methods.
            SomePrivateMethod();

            // This is a test to ensure the analyzer DOES raise on public member methods.
            SomePublicMethod();

            // This is a test to ensure the analyzer DOES raise on protected member methods.

            // This is a test to ensure the analyzer DOES raise on private static member methods.
            SomePublicStaticMethod();

            // This is a test to ensure the analyzer DOES raise on private static member methods.
            SomePrivateStaticMethod();
        }

        /// <summary>
        /// A public method to test the analyzer.
        /// </summary>
        /// <example>
        /// <code>
        /// ConstructorExternalMethodInvocationProof.SomePublicStaticMethod();
        /// </code>
        /// </example>
        public static void SomePublicStaticMethod()
        {
        }

        /// <summary>
        /// A public method to test the analyzer.
        /// </summary>
        /// <example>
        /// <code>
        /// ConstructorExternalMethodInvocationProof.SomePublicMethod();
        /// </code>
        /// </example>
        public void SomePublicMethod()
        {
            if (_lazyLambaTestShouldNotRaise.IsValueCreated)
            {
                return;
            }
        }

        private void SomePrivateStaticMethod()
        {
        }

        private void SomePrivateMethod()
        {
            if (_lazyLambaTestShouldNotRaise.IsValueCreated)
            {
                return;
            }
        }
    }
}
