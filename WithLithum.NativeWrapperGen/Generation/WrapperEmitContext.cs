namespace WithLithum.NativeWrapperGen.Generation;

using WithLithum.NativeWrapperGen.Models;

public readonly record struct WrapperEmitContext
{
    public required string Hash { get; init; }
    public required string SymbolNameHash { get; init; }
    public required ScriptCommandInfo CommandInfo { get; init; }
    public required string ReturnTypeString { get; init; }
}
