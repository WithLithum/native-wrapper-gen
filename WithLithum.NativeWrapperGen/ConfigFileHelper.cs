namespace WithLithum.NativeWrapperGen;

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using WithLithum.NativeWrapperGen.Models;
using WithLithum.NativeWrapperGen.Serialization;

internal static class ConfigFileHelper
{
    private static readonly string ProgramRootPath = Path.GetDirectoryName(Environment.ProcessPath)
        ?? "\\";

    private static readonly string ProgramDataPath = Path.GetFullPath("Data", ProgramRootPath);

    private static T? LoadFileInternal<T>(string filePath,
        JsonTypeInfo<T> typeInfo)
    {
        if (!File.Exists(filePath))
        {
            return default;
        }

        using var fs = File.OpenRead(filePath);

        return JsonSerializer.Deserialize(fs, typeInfo);
    }

    private static T? LoadDataInternal<T>(string fileName,
        JsonTypeInfo<T> typeInfo)
    {
        var filePath = Path.GetFullPath(fileName, ProgramDataPath);
        return LoadFileInternal(filePath, typeInfo);
    }

    internal static ScriptCommandManifest? LoadManifest(string? customFile = null)
    {
        if (!string.IsNullOrWhiteSpace(customFile))
        {
            return LoadFileInternal(customFile, ScriptCommandInfoContext.Default.ScriptCommandManifest);
        }

        return LoadDataInternal("BundledNativeInfo.json",
            ScriptCommandInfoContext.Default.ScriptCommandManifest);
    }

    internal static GeneratorSettings? LoadShvdnSettings(string? customFile = null)
    {
        if (!string.IsNullOrWhiteSpace(customFile))
        {
            return LoadFileInternal(customFile, ScriptCommandInfoContext.Default.GeneratorSettings);
        }

        return LoadDataInternal("SHVDNSettings.json",
            ScriptCommandInfoContext.Default.GeneratorSettings);
    }
}
