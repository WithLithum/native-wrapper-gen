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
namespace WithLithum.NativeWrapperGen.Models;

using System.Text.Json.Serialization;

public record ScriptCommandInfo
{
    public string? Name { get; init; }
    
    /// <summary>
    /// Gets the override symbol name of the script command that will be emitted. This property is
    /// a Native Wrapper Generator extension.
    /// </summary>
    public string? SymbolName { get; init; }

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
