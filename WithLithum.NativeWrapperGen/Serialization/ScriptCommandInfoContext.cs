// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;
using WithLithum.NativeWrapperGen.Models;
using WithLithum.NativeWrapperGen.Models.Settings;

namespace WithLithum.NativeWrapperGen.Serialization;

[JsonSerializable(typeof(ScriptCommandParameterType))]
[JsonSerializable(typeof(ScriptCommandParameterInfo))]
[JsonSerializable(typeof(ScriptCommandReturnType))]
[JsonSerializable(typeof(ScriptCommandInfo))]
[JsonSerializable(typeof(ScriptCommandManifest), TypeInfoPropertyName = "ScriptCommandManifest")]
[JsonSerializable(typeof(GeneratorTypeSettings))]
[JsonSerializable(typeof(GeneratorSettings))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
internal partial class ScriptCommandInfoContext : JsonSerializerContext
{
}
