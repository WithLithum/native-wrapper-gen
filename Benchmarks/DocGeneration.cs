using BenchmarkDotNet.Attributes;
using WithLithum.NativeWrapperGen.Generation;
using WithLithum.NativeWrapperGen.Generation.Hooks;
using WithLithum.NativeWrapperGen.Models;

namespace Benchmarks;

[MemoryDiagnoser]
public class DocGeneration
{
    private static readonly ScriptCommandInfo SampleData = new()
    {
        Name = "TEST_COMMAND",
        JenkinsHash = "0x13456771",
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
        OldNames = ["OLD_NAME_1", "OLD_NAME_2"],
        Build = "b323"
    };

    [Benchmark]
    public void EscapeWrite()
    {
        DocGenerator.WriteEscapedForDocumentation("<escape/me>", TextWriter.Null);
    }

    [Benchmark]
    public void GenerateDoc()
    {
        var context = new WrapperEmitContext
        {
            CommandInfo = SampleData,
            Hash = "0x1234567890ABCDEF",
            ReturnTypeString = "void",
            Namespace = "TEST",
        };
        DocGenerator.WriteDocumentation(context, TextWriter.Null, VDotNetGenerator.DefaultSettings);
    }
}
