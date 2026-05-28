// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using WithLithum.NativeWrapperGen.Generation.Hooks;
using WithLithum.NativeWrapperGen.Models;
using WithLithum.NativeWrapperGen.Serialization;

namespace WithLithum.NativeWrapperGen;

public static class ConfigFileHelper
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

    public static GeneratorSettings? LoadSettingsFile(string fileName)
    {
        return LoadFileInternal(fileName, ScriptCommandInfoContext.Default.GeneratorSettings);
    }

    internal static GeneratorSettings? GetDefault(string generator)
    {
        return generator switch
        {
            VDotNetGenerator.Id => VDotNetGenerator.DefaultSettings,
            RageGenerator.Id => RageGenerator.DefaultSettings,
            _ => null,
        };
    }

}
