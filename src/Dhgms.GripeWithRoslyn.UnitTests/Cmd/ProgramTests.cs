// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NET48
using System;
using System.IO;
using System.Threading.Tasks;
using Dhgms.GripeWithRoslyn.Cmd;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Dhgms.GripeWithRoslyn.UnitTests.Cmd
{
    /// <summary>
    /// Unit Tests for <see cref="Program" />.
    /// </summary>
    public static class ProgramTests
    {
        /// <summary>
        /// Unit Tests for <see cref="Program.Main(string[])" />.
        /// </summary>
        public sealed class MainMethod : Foundatio.Xunit.TestWithLoggingBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MainMethod"/> class.
            /// </summary>
            /// <param name="output">XUnit logging output helper.</param>
            public MainMethod(ITestOutputHelper output)
                : base(output)
            {
            }

            /// <summary>
            /// Test to ensure a failure run non zero.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
            [Fact]
            public async Task ReturnsNonZero()
            {
                var args = new[]
                {
                    "C:\\GitHub\\dpvreony\\whipstaff\\src\\whipstaff.sln"
                };

                var textwriter = new StringWriter();
                Console.SetOut(textwriter);
                var result = await Program.Main(args);

                var stdOutputString = textwriter.ToString();

                _logger.LogInformation(stdOutputString);

                Assert.NotEqual(0, result);
            }
        }
    }
}
#endif