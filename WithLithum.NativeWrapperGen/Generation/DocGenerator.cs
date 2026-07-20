// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Immutable;
using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation;

public static class DocGenerator
{
    public static void WriteEscapedForDocumentation(string comment,
        TextWriter writer)
    {
        var buf = comment.AsSpan();
        char c;
        string? escape;
        for (var i = 0; i < buf.Length; i++)
        {
            c = buf[i];

            escape = c switch
            {
                '<' => "&lt;",
                '>' => "&gt;",
                '&' => "&amp;",
                '\"' => "&quot;",
                '\'' => "&apos;",
                '\n' => "<br />",
                _ => null
            };

            if (escape != null)
            {
                writer.Write(escape);
            }
            else
            {
                writer.Write(c);
            }
        }
    }

    private static void WriteRemarkEntry(string property,
        string value,
        TextWriter writer)
    {
        // Property name
        writer.Write("/// <b>");
        WriteEscapedForDocumentation(property, writer);
        writer.Write("</b>: ");

        WriteEscapedForDocumentation(value, writer);
        writer.WriteLine("<br />");
    }

    private static void WritePreviouslyKnownAs(ScriptCommandInfo commandInfo,
        TextWriter writer)
    {
        if (commandInfo.OldNames == null || commandInfo.OldNames.Count == 0)
        {
            return;
        }

        writer.WriteLine("/// <para><b>Previously known as</b>:<br />");
        foreach (var oldName in commandInfo.OldNames)
        {
            writer.Write("/// <c>");
            WriteEscapedForDocumentation(oldName, writer);
            writer.WriteLine("</c>");
        }
        writer.WriteLine("/// </para>");
    }

    public static void WriteDocumentation(in WrapperEmitContext context,
        TextWriter writer,
        GeneratorSettings settings)
    {
        var commandInfo = context.CommandInfo;

        // Summary
        if (!string.IsNullOrWhiteSpace(commandInfo.Comment))
        {
            writer.WriteLine("/// <summary>");

            writer.Write("/// ");
            WriteEscapedForDocumentation(commandInfo.Comment, writer);
            writer.WriteLine();

            writer.WriteLine("/// </summary>");
        }

        // Parameters
        foreach (var param in commandInfo.Parameters)
        {
            writer.Write("/// <param name=\"");
            writer.Write(param.Name);
            writer.Write("\"><c>");
            writer.Write(param.Type.ToString());
            writer.WriteLine("</c></param>");
        }

        // Remarks
        writer.WriteLine("/// <remarks>");

        WriteRemarkEntry("Namespace", context.Namespace, writer);
        WriteRemarkEntry("Introduced in", commandInfo.Build, writer);
        WriteRemarkEntry("PC hash", context.Hash, writer);
        if (!string.IsNullOrWhiteSpace(commandInfo.JenkinsHash))
        {
            WriteRemarkEntry("Original hash", commandInfo.JenkinsHash, writer);
        }
        WritePreviouslyKnownAs(commandInfo, writer);

        writer.WriteLine("/// </remarks>");

        // Returns
        // Only write when it indeed has a return value.
        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            writer.Write("/// <returns><c>");
            writer.Write(commandInfo.ReturnType.ToString());
            writer.WriteLine("</c></returns>");
        }
    }
}
