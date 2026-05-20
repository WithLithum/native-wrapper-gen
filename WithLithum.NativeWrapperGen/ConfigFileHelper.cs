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
