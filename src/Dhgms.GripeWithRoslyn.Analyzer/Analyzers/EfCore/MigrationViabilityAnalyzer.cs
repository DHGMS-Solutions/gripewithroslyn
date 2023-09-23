// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Dhgms.GripeWithRoslyn.Analyzer.Analyzers.EfCore
{
    /// <summary>
    /// This checks for the minimum requirements for a migration to be viable.
    /// This is to prevent risks where people can forget to check in the designer file
    /// or make breaking changes to it, that don't get picked up by the build or eve
    /// the application of the migration itself, because it can be skipped.
    /// </summary>
    public sealed class MigrationViabilityAnalyzer
    {
    }
}
