// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Dhgms.GripeWithRoslyn.Analyzer.CodeCracker.Extensions
{
    /// <summary>
    /// Represents the different categories you can designate your issue as during analysis.
    /// </summary>
    public static class SupportedCategories
    {
        /// <summary>
        /// Represents a design issue.
        /// </summary>
        public const string Design = nameof(Design);

        /// <summary>
        /// Represents a globalization issue.
        /// </summary>
        public const string Globalization = nameof(Globalization);

        /// <summary>
        /// Represents a design issue.
        /// </summary>
        public const string Interoperability = nameof(Interoperability);

        /// <summary>
        /// Represents a maintainability issue.
        /// </summary>
        public const string Maintainability = nameof(Maintainability);

        /// <summary>
        /// Represents a mobility issue.
        /// </summary>
        public const string Mobility = nameof(Mobility);

        /// <summary>
        /// Represents a naming issue.
        /// </summary>
        public const string Naming = nameof(Naming);

        /// <summary>
        /// Represents a performance issue.
        /// </summary>
        public const string Performance = nameof(Performance);

        /// <summary>
        /// Represents a portability issue.
        /// </summary>
        public const string Portability = nameof(Portability);

        /// <summary>
        /// Represents a refactoring issue.
        /// </summary>
        public const string Refactoring = nameof(Refactoring);

        /// <summary>
        /// Represents a reliability issue.
        /// </summary>
        public const string Reliability = nameof(Reliability);

        /// <summary>
        /// Represents a security issue.
        /// </summary>
        public const string Security = nameof(Security);

        /// <summary>
        /// Represents a style issue.
        /// </summary>
        public const string Style = nameof(Style);

        /// <summary>
        /// Represents a usage issue.
        /// </summary>
        public const string Usage = nameof(Usage);
    }
}