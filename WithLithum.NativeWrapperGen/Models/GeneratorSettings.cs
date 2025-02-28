namespace WithLithum.NativeWrapperGen.Models;

public record GeneratorSettings
{
    public required ReturnTypeConversionTable ReturnTypes { get; init; }
    public required ParameterTypeConversionTable ParameterTypes { get; init; }
    public required string Accessibility { get; init; }
}
