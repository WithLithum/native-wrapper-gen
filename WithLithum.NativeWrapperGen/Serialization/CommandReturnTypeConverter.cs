namespace WithLithum.NativeWrapperGen.Serialization;

using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using WithLithum.NativeWrapperGen.Models;

public class CommandReturnTypeConverter : JsonConverter<ScriptCommandReturnType>
{
    private static readonly ReadOnlyDictionary<string, ScriptCommandReturnType> SpecialTreatmentStrings = 
        new Dictionary<string, ScriptCommandReturnType>()
        {
            { "Any*", ScriptCommandReturnType.AnyPointer },
            { "const char*", ScriptCommandReturnType.String },
            { "BOOL", ScriptCommandReturnType.Boolean }
        }.AsReadOnly();

    public override ScriptCommandReturnType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string but found {reader.TokenType}");
        }

        string? str = reader.GetString();
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new JsonException($"Expected string but found null, empty or whitespace");
        }

        if (SpecialTreatmentStrings.TryGetValue(str, out var specialValue))
        {
            return specialValue;
        }

        if (Enum.TryParse<ScriptCommandReturnType>(str, true, out var normalValue))
        {
            return normalValue;
        }

        throw new JsonException($"Expected return value type but got \"{str}\"");
    }

    public override void Write(Utf8JsonWriter writer, ScriptCommandReturnType value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    public override ScriptCommandReturnType ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? str = reader.GetString();
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new JsonException($"Expected string but found null, empty or whitespace");
        }

        if (!Enum.TryParse<ScriptCommandReturnType>(str, true, out var value))
        {
            throw new JsonException($"Expected return value type but got \"{str}\"");
        }

        return value;
    }
}
