// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using BenchmarkDotNet.Attributes;
using WithLithum.NativeWrapperGen.Generation;
using WithLithum.NativeWrapperGen.Generation.Hooks;
using WithLithum.NativeWrapperGen.Models;

namespace Benchmarks;

public class GenerateToNull
{
    private static readonly IReadOnlyDictionary<string, ScriptCommandInfo> SampleData =
        new Dictionary<string, ScriptCommandInfo>
        {
            ["0x1234567890abcdef"] = new ScriptCommandInfo
            {
                Name = "TEST_COMMAND",
                Parameters =
                    [
                        new ScriptCommandParameterInfo
                        {
                            Name = "param1",
                            Type = ScriptCommandParameterType.Int
                        },
                        new ScriptCommandParameterInfo
                        {
                            Name = "parma2",
                            Type = ScriptCommandParameterType.Float
                        }
                    ],
                ReturnType = ScriptCommandReturnType.Void,
                Build = "b323"
            }
        };

    private readonly WrapperFileGenerator _generator;

    public GenerateToNull()
    {
        _generator = new WrapperFileGenerator(VDotNetGenerator.DefaultSettings,
            new VDotNetGenerator(VDotNetGenerator.DefaultSettings));
    }

    [Benchmark]
    public void GenerateSample()
    {
        var context = new WrapperSectionContext
        {
            Namespace = "NS",
            Commands = SampleData
        };
        _generator.WritePartial("TestNamespace", "TestClass", in context,
            TextWriter.Null);
    }

}
