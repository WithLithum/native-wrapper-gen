// SPDX-FileCopyrightText:2025-2026 WithLithum
// SPDX-License-Identifier:Apache-2.0

namespace WithLithum.NativeWrapperGen.Models.Settings;

public enum HashNameStyle
{
    /// <summary>
    /// Change the hex specifier from <c>0x</c> to a single <c>x</c> character. This is the style
    /// used for invoking natives by hash, via <c>NativeFunction.Natives</c>, on RAGE Plugin Hook.
    /// </summary>
    RagePluginHook,
    /// <summary>
    /// Prefix the hash name with <c>N_</c>. This is the style used for the default native wrappers
    /// on CitizenFX scripting runtimes.
    /// </summary>
    Cfx
}
