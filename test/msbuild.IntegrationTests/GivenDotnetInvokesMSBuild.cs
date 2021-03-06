﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.DotNet.Tools.Test.Utilities;
using Xunit;
using System;
using System.IO;
using FluentAssertions;
using Microsoft.DotNet.TestFramework;
using Microsoft.DotNet.Cli.Utils;

namespace Microsoft.DotNet.Cli.MSBuild.IntegrationTests
{
    public class GivenDotnetInvokesMSBuild : TestBase
    {
        [Theory]
        [InlineData("build")]
        [InlineData("clean")]
        [InlineData("msbuild")]
        [InlineData("pack")]
        [InlineData("publish")]
        [InlineData("test")]
        public void When_dotnet_command_invokes_msbuild_Then_env_vars_and_m_are_passed(string command)
        {
            var testInstance = TestAssets.Get("MSBuildIntegration")
                .CreateInstance(identifier: command)
                .WithSourceFiles();

            new DotnetCommand()
                .WithWorkingDirectory(testInstance.Root)
                .Execute(command)
                .Should().Pass();
        }

        [Theory]
        [InlineData("build")]
        [InlineData("clean")]
        [InlineData("pack")]
        [InlineData("publish")]
        public void When_dotnet_command_invokes_msbuild_with_no_args_verbosity_is_set_to_minimum(string command)
        {
            var testInstance = TestAssets.Get("MSBuildIntegration")
                .CreateInstance(identifier: command)
                .WithSourceFiles();

            var cmd = new DotnetCommand()
                .WithWorkingDirectory(testInstance.Root)
                .ExecuteWithCapturedOutput(command);
            cmd.Should().Pass();
            cmd.StdOut.Should().NotContain("Message with normal importance");
            // sanity check
            cmd.StdOut.Should().Contain("Message with high importance");
        }

        [Theory]
        [InlineData("build")]
        [InlineData("clean")]
        [InlineData("pack")]
        [InlineData("publish")]
        [InlineData("test")]
        public void When_dotnet_command_invokes_msbuild_with_diag_verbosity_Then_arg_is_passed(string command)
        {
            var testInstance = TestAssets.Get("MSBuildIntegration")
                .CreateInstance(identifier: command)
                .WithSourceFiles();

            var cmd = new DotnetCommand()
                .WithWorkingDirectory(testInstance.Root)
                .ExecuteWithCapturedOutput($"{command} -v diag");
            cmd.Should().Pass();
            cmd.StdOut.Should().Contain("Message with low importance");
        }

        [Fact]
        public void When_dotnet_test_invokes_msbuild_with_no_args_verbosity_is_set_to_quiet()
        {
            string command = "test";
            var testInstance = TestAssets.Get("MSBuildIntegration")
                .CreateInstance(identifier: command)
                .WithSourceFiles();

            var cmd = new DotnetCommand()
                .WithWorkingDirectory(testInstance.Root)
                .ExecuteWithCapturedOutput(command);
            cmd.Should().Pass();
            cmd.StdOut.Should().NotContain("Message with high importance");
        }

        [Fact]
        public void When_dotnet_msbuild_command_is_invoked_with_non_msbuild_switch_Then_it_fails()
        {
            string command = "msbuild";
            var testInstance = TestAssets.Get("MSBuildIntegration")
                .CreateInstance(identifier: command)
                .WithSourceFiles();

            var cmd = new DotnetCommand()
                .WithWorkingDirectory(testInstance.Root)
                .ExecuteWithCapturedOutput($"{command} -v diag");
            cmd.ExitCode.Should().NotBe(0);
        }
    }
}
