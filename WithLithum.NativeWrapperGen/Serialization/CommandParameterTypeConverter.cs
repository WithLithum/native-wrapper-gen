namespace WithLithum.NativeWrapperGen.Serialization;

using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using WithLithum.NativeWrapperGen.Models;

public class CommandParameterTypeConverter : JsonConverter<ScriptCommandParameterType>
{
    private static readonly ReadOnlyDictionary<string, ScriptCommandParameterType> SpecialTreatmentStrings =
        new Dictionary<string, ScriptCommandParameterType>()
        {
            { "Any*", ScriptCommandParameterType.AnyPointer },
            { "char*", ScriptCommandParameterType.MutableString },
            { "const char*", ScriptCommandParameterType.String },
            { "BOOL", ScriptCommandParameterType.Boolean },
            { "BOOL*", ScriptCommandParameterType.BooleanPointer },
            { "int*", ScriptCommandParameterType.IntPointer },
            { "float*", ScriptCommandParameterType.FloatPointer },
            { "Blip*", ScriptCommandParameterType.BlipPointer },
            { "Cam*", ScriptCommandParameterType.CamPointer },
            { "Entity*", ScriptCommandParameterType.EntityPointer },
            { "FireId*", ScriptCommandParameterType.FireIdPointer },
            { "Hash*", ScriptCommandParameterType.HashPointer },
            { "Interior*", ScriptCommandParameterType.InteriorPointer },
            { "ItemSet*", ScriptCommandParameterType.ItemSetPointer },
            { "Object*", ScriptCommandParameterType.ObjectPointer },
            { "Ped*", ScriptCommandParameterType.PedPointer },
            { "Pickup*", ScriptCommandParameterType.PickupPointer },
            { "Player*", ScriptCommandParameterType.PlayerPointer },
            { "ScrHandle*", ScriptCommandParameterType.ScrHandlePointer },
            { "Vector3*", ScriptCommandParameterType.Vector3Pointer },
            { "Vehicle*", ScriptCommandParameterType.VehiclePointer }
        }.AsReadOnly();

    public override ScriptCommandParameterType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        if (Enum.TryParse<ScriptCommandParameterType>(str, true, out var normalValue))
        {
            return normalValue;
        }

        throw new JsonException($"Expected parameter value type but got \"{str}\"");
    }

    public override void Write(Utf8JsonWriter writer, ScriptCommandParameterType value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    public override ScriptCommandParameterType ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? str = reader.GetString();
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new JsonException($"Expected string but found null, empty or whitespace");
        }

        if (!Enum.TryParse<ScriptCommandParameterType>(str, true, out var value))
        {
            throw new JsonException($"Expected parameter value type but got \"{str}\"");
        }

        return value;
    }
}
