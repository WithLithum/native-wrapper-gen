// SPDX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation;

public readonly ref struct WrapperSectionContext
{
    public required string Namespace { get; init; }

    public required IReadOnlyDictionary<string, ScriptCommandInfo> Commands { get; init; }
}
