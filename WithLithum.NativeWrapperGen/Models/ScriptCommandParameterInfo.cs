namespace WithLithum.NativeWrapperGen.Models;

public readonly record struct ScriptCommandParameterInfo
{
    public required string Name { get; init; }
    public required ScriptCommandParameterType Type { get; init; }
}
