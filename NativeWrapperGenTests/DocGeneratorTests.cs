// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Generation;

namespace NativeWrapperGenTests;

public class DocGeneratorTests
{
    [Fact]
    public void WriteEscaped_Comment_EscapeCorrectly()
    {
        // Arrange
        const string input = "This is a comment with a <tag> and a \n newline.";
        var writer = new StringWriter();

        // Act
        DocGenerator.WriteEscapedForDocumentation(input, writer);
        
        // Assert
        Assert.Equal("This is a comment with a &lt;tag&gt; and a <br /> newline.",
            writer.ToString());
    }
}