namespace WithLithum.NativeWrapperGen.Serialization;

using System.Text.Json.Serialization;
using WithLithum.NativeWrapperGen.Models;

[JsonSerializable(typeof(ScriptCommandParameterType))]
[JsonSerializable(typeof(ScriptCommandParameterInfo))]
[JsonSerializable(typeof(ScriptCommandReturnType))]
[JsonSerializable(typeof(ScriptCommandInfo))]
[JsonSerializable(typeof(ScriptCommandManifest), TypeInfoPropertyName = "ScriptCommandManifest")]
[JsonSerializable(typeof(GeneratorSettings))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
internal partial class ScriptCommandInfoContext : JsonSerializerContext
{
}
