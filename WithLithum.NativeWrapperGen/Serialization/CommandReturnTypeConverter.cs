// Copyright (C) 2025 WithLithum.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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
