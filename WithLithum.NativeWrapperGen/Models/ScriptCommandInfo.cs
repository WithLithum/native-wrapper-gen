namespace WithLithum.NativeWrapperGen.Models;

using System.Text.Json.Serialization;

public record ScriptCommandInfo
{
    public string? Name { get; init; }

    [JsonPropertyName("jhash")]
    public string? JenkinsHash { get; init; }

    public string? Comment { get; init; }

    [JsonPropertyName("params")]
    public required IReadOnlyList<ScriptCommandParameterInfo> Parameters { get; init; }

    public required ScriptCommandReturnType ReturnType { get; init; }

    public required string Build { get; init; }

    public bool Unused { get; init; }

    public IReadOnlyList<string>? OldNames { get; init; }
}
