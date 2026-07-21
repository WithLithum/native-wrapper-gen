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
using System.Buffers;

namespace WithLithum.NativeWrapperGen.Generation;

public static class MethodNameConverter
{
    public const int MaximumCharacterLength = 150;

    public static string HashToMethodName(string from)
    {
        return from[1..];
    }

    public static int SnakeToPascal(in ReadOnlySpan<char> input,
        in Span<char> buffer)
    {
        var isCaptialize = true;
        var writeIndex = 0;

        // fromSpan and toSpan have the same length except when fromSpan exceeds,
        // MaximumCharacterLength. In that case, the length is MaximumCharacterLength.
        for (int i = 0; i < buffer.Length; i++)
        {
            var ch = input[i];
            if (ch == '_') // Underscore
            {
                isCaptialize = true;
                continue;
            }

            // Transform the character to write.

            char writeCh;

            if (isCaptialize)
            {
                writeCh = char.ToUpperInvariant(ch);
                isCaptialize = false;
            }
            else
            {
                writeCh = char.ToLowerInvariant(ch);
            }

            // Write the character and advance the write index.
            buffer[writeIndex++] = writeCh;
        }

        return writeIndex;
    }

    public static string SnakeToPascal(string from)
    {
        ReadOnlySpan<char> fromSpan = from.AsSpan();
        Span<char> toSpan = fromSpan.Length >= MaximumCharacterLength
            ? ArrayPool<char>.Shared.Rent(fromSpan.Length)
            : stackalloc char[fromSpan.Length];

        var writeIndex = SnakeToPascal(fromSpan, toSpan);

        return new string(toSpan[..writeIndex]);
    }
}
