// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Generation;
using WithLithum.NativeWrapperGen.Models;

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

    [Fact]
    public void WriteParameter_WithNoComment_WriteType()
    {
        // Arrange
        var param = new ScriptCommandParameterInfo
        {
            Name = "parameter",
            Type = ScriptCommandParameterType.Int
        };
        var writer = new StringWriter();

        // Act
        DocGenerator.WriteParameter(writer, param);

        // Assert
        Assert.Equal($"/// <param name=\"parameter\"><c>Int</c></param>{Environment.NewLine}",
            writer.ToString());
    }

    [Fact]
    public void WriteParameter_WithComment_WriteComment()
    {
        // Arrange
        var param = new ScriptCommandParameterInfo
        {
            Name = "parameter",
            Comment = "This is a comment",
            Type = ScriptCommandParameterType.Int
        };
        var writer = new StringWriter();

        // Act
        DocGenerator.WriteParameter(writer, param);

        // Assert
        Assert.Equal($"/// <param name=\"parameter\">This is a comment</param>{Environment.NewLine}",
            writer.ToString());
    }
}
