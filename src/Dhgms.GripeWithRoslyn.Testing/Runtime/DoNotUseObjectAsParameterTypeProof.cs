// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.Runtime;

namespace Dhgms.GripeWithRoslyn.Testing.Runtime
{
    /// <summary>
    /// Analyzer proof for <see cref="DoNotUseObjectAsParameterTypeAnalyzer"/>.
    /// </summary>
    public sealed class DoNotUseObjectAsParameterTypeProof : WeakReference, IComparer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseObjectAsParameterTypeProof"/> class.
        /// </summary>
        /// <param name="target">The object to track or <see langword="null" />.</param>
        /// <example>
        /// <code>
        /// var target = new object();
        /// var proof = new DoNotUseObjectAsParameterTypeProof(target);
        /// </code>
        /// </example>
        public DoNotUseObjectAsParameterTypeProof(object? target)
            : base(target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseObjectAsParameterTypeProof"/> class.
        /// </summary>
        /// <param name="target">An object to track.</param>
        /// <param name="trackResurrection">Indicates when to stop tracking the object. If <see langword="true" />, the object is tracked after finalization; if <see langword="false" />, the object is only tracked until finalization.</param>
        /// <example>
        /// <code>
        /// var target = new object();
        /// var proof = new DoNotUseObjectAsParameterTypeProof(target, false);
        /// </code>
        /// </example>
        public DoNotUseObjectAsParameterTypeProof(
            object? target,
            bool trackResurrection)
            : base(
                target,
                trackResurrection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotUseObjectAsParameterTypeProof"/> class.
        /// </summary>
        /// <param name="target">An object to track.</param>
        /// <param name="trackResurrection">Indicates when to stop tracking the object. If <see langword="true" />, the object is tracked after finalization; if <see langword="false" />, the object is only tracked until finalization.</param>
        /// <param name="someExtraObject">An extra object used to check a warning is created.</param>
        /// <example>
        /// <code>
        /// var target = new object();
        /// var someExtraObject = new object();
        /// var proof = new DoNotUseObjectAsParameterTypeProof(target, false, someExtraObject);
        /// </code>
        /// </example>
        public DoNotUseObjectAsParameterTypeProof(
            object? target,
            bool trackResurrection,
            object someExtraObject)
            : base(
                target,
                trackResurrection)
        {
            ArgumentNullException.ThrowIfNull(someExtraObject);
        }

        /// <summary>
        /// Method to ensure <see cref="object"/> is not used in a parameter declaration.
        /// </summary>
        /// <param name="arg">The object arg.</param>
        /// <example>
        /// <code>
        /// var target = new object();
        /// var proof = new DoNotUseObjectAsParameterTypeProof(target);
        /// var arg = new object();
        /// proof.MethodName(arg);
        /// </code>
        /// </example>
        public void MethodName(object arg)
        {
        }

#pragma warning disable SA1121 // Use built-in type alias
        /// <summary>
        /// Method to ensure <see cref="object"/> is not used in a parameter declaration.
        /// </summary>
        /// <param name="arg">The object arg.</param>
        /// <example>
        /// <code>
        /// var target = new object();
        /// var proof = new DoNotUseObjectAsParameterTypeProof(target);
        /// var x = new object();
        /// var y = new object();
        /// proof.MethodName(x, y);
        /// </code>
        /// </example>
        public void MethodName2(System.Object arg)
#pragma warning restore SA1121 // Use built-in type alias
        {
        }

        /// <inheritdoc/>
        public int Compare(object? x, object? y)
        {
            throw new NotImplementedException();
        }
    }
}
