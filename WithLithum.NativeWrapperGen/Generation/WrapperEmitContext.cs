// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation;

public readonly ref struct WrapperEmitContext
{
    public required string Hash { get; init; }
    public required string SymbolNameHash { get; init; }
    public required ScriptCommandInfo CommandInfo { get; init; }
    public required string ReturnTypeString { get; init; }
}
