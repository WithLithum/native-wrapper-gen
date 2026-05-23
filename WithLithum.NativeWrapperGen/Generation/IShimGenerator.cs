// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

namespace WithLithum.NativeWrapperGen.Generation;

public interface ShimGenerator
{
    void WriteMethod(in WrapperEmitContext context, TextWriter writer);
}