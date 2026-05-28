// SPDX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Models.Settings;

namespace WithLithum.NativeWrapperGen.Models;

/// <summary>
/// Configures the behaviour of shim generators.
/// </summary>
public record GeneratorSettings
{
    /// <summary>
    /// Gets the type mapping configuration.
    /// </summary>
    public required GeneratorTypeSettings TypeSettings { get; init; }

    /// <summary>
    /// Gets the accessibility modifier to use for generated symbols.
    /// </summary>
    public required string Accessibility { get; init; }
}
