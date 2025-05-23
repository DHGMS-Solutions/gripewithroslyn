﻿// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace Gripe.MSBuildBinLogStats.DotNetTool.CommandLine
{
    /// <summary>
    /// Model that represents the command line arguments.
    /// </summary>
    public sealed record CommandLineArgModel(FileInfo BinLogPath);
}
