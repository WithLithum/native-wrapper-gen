// SPDX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Models.Settings;

namespace WithLithum.NativeWrapperGen.Models;

public record GeneratorSettings
{
    public required GeneratorTypeSettings TypeSettings { get; init; }

    public required string Accessibility { get; init; }
}
