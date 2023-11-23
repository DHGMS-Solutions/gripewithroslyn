// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;

namespace Dhgms.GripeWithRoslyn.Cmd.CommandLine
{
    internal static class ArgumentExtensions
    {
        internal static Argument<FileInfo> SpecificFileExtensionOnly(this Argument<FileInfo> argument, string extension)
        {
            argument.AddValidator(result => FileHasSupportedExtension(result, extension));
            return argument;
        }

        internal static Argument<FileInfo> SpecificFileExtensionOnly(this Argument<FileInfo> argument, string[] extensions)
        {
            argument.AddValidator(result => FileHasSupportedExtension(result, extensions));
            return argument;
        }

        internal static void FileHasSupportedExtension(ArgumentResult result, string extension)
        {
            for (var i = 0; i < result.Tokens.Count; i++)
            {
                var token = result.Tokens[i];

                var tokenExtension = Path.GetExtension(token.Value);

                if (string.IsNullOrWhiteSpace(tokenExtension)
                    || !tokenExtension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                {
                    result.ErrorMessage = $"Filename does not have a supported extension of \"{extension}\".";
                    return;
                }
            }
        }

        internal static void FileHasSupportedExtension(ArgumentResult result, string[] extensions)
        {
            for (var i = 0; i < result.Tokens.Count; i++)
            {
                var token = result.Tokens[i];

                var tokenExtension = Path.GetExtension(token.Value);

                if (string.IsNullOrWhiteSpace(tokenExtension)
                    || !extensions.Any(extension => tokenExtension.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    result.ErrorMessage = $"Filename does not have a supported extension of \"{string.Join(",", extensions)}\".";
                    return;
                }
            }
        }
    }
}
