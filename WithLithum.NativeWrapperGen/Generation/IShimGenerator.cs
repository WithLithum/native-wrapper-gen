// SPDX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

namespace WithLithum.NativeWrapperGen.Generation;

/// <summary>
/// Defines the interface of a shim generator implementation.
/// </summary>
/// <remarks>
/// <note type="implement">
/// The implementation should not maintain state between calls to
/// <see cref="WriteMethod(in WrapperEmitContext, TextWriter)"/>, as the same instance will be
/// reused.
/// </note>
/// </remarks>
public interface IShimGenerator
{
    void WriteMethod(in WrapperEmitContext context, TextWriter writer);
}
