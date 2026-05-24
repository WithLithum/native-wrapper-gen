// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using BenchmarkDotNet.Attributes;
using WithLithum.NativeWrapperGen.Generation;
using WithLithum.NativeWrapperGen.Generation.Hooks;
using WithLithum.NativeWrapperGen.Models;

namespace Benchmarks;

public class VDotNetGenerateToNull
{
    private static readonly GeneratorSettings Settings =
        new()
        {
            ReturnTypes = new Dictionary<ScriptCommandReturnType, string>
            {
                { ScriptCommandReturnType.Void, "void" }
            },
            ParameterTypes = new Dictionary<ScriptCommandParameterType, string>
            {
                { ScriptCommandParameterType.Int, "int" },
                { ScriptCommandParameterType.Float, "float" }
            },
            Accessibility = "public"
        };

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

    public VDotNetGenerateToNull()
    {
        _generator = new WrapperFileGenerator(TextWriter.Null,
            Settings,
            new VDotNetGenerator(Settings));
    }

    [Benchmark]
    public void GenerateSample()
    {
        _generator.WritePartial("TestNamespace", "TestClass", SampleData);
    }

}
