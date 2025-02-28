namespace WithLithum.NativeWrapperGen.Generation;

public static class MethodNameConverter
{
    public const int MaximumCharacterLength = 150;

    public static string HashToMethodName(string from)
    {
        return from[1..];
    }

    public static string SnakeToPascal(string from)
    {
        ReadOnlySpan<char> fromSpan = from.AsSpan();
        Span<char> toSpan = fromSpan.Length >= MaximumCharacterLength
            ? stackalloc char[MaximumCharacterLength]
            : stackalloc char[from.Length];

        var isCaptialize = true;
        var writeIndex = 0;

        // fromSpan and toSpan have the same length except when fromSpan exceeds,
        // MaximumCharacterLength. In that case, the length is MaximumCharacterLength.
        for (int i = 0; i < toSpan.Length; i++)
        {
            var ch = fromSpan[i];
            if (ch == '_') // Underscore
            {
                isCaptialize = true;
                continue;
            }

            // Transform the character to write.

            char writeCh = ch;

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

            toSpan[writeIndex] = writeCh;
            writeIndex++;
        }

        return new string(toSpan[..writeIndex]);
    }
}
