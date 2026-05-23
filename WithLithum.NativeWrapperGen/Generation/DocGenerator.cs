// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Immutable;
using System.Security;
using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation;

public static class DocGenerator
{
    private static readonly ImmutableList<char> EscapeXmlCharacters =
    [
        '<',
        '>',
        '&',
        '"',
        '\''
    ];
    
    private static string GetStringForType(ScriptCommandParameterType paramType,
        GeneratorSettings settings,
        bool stripRef = false)
    {
        while (true)
        {
            // ReSharper disable once InvertIf
            if (stripRef && ParamUtil.PointerToRegularMap.TryGetValue(paramType, out var resultType))
            {
                paramType = resultType;
                stripRef = false;
                continue;
            }

            return settings.ParameterTypes.TryGetValue(paramType, out var writeType)
                ? writeType
                : paramType.ToString();
        }
    }

    public static void WriteEscapedForDocumentation(string comment,
        TextWriter writer)
    {
        var buf = comment.AsSpan();
        for (var i = 0; i < buf.Length; i++)
        {
            var c = buf[i];
            if (EscapeXmlCharacters.Contains(c))
            {
                writer.Write(c switch
                {
                    '<' => "&lt;",
                    '>' => "&gt;",
                    '&' => "&amp;",
                    '\"' => "&quot;",
                    '\'' => "&apos;",
                    _ => c.ToString() // Normally won't reach this
                });
                continue;
            }

            if (c == '\\')
            {
                var nextI = i + 1;
                if (nextI < buf.Length && buf[nextI] == 'n')
                {
                    writer.Write("<br />");
                    i = nextI;
                }

                continue;
            }

            writer.Write(c);
        }
    }
    
    private static void WriteRemarkEntry(string property,
        string value,
        TextWriter writer)
    {
        // Property name
        writer.Write("/// <b>");
        writer.Write(SecurityElement.Escape(property));
        writer.Write("</b>: ");

        writer.Write(SecurityElement.Escape(value));
        writer.WriteLine("<br />");
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

            writer.WriteLine("/// </summary>");
        }

        // Parameters
        foreach (var param in commandInfo.Parameters)
        {
            writer.Write("/// <param name=\"");
            writer.Write(param.Name);
            writer.Write("\">An instance of <c>");
            writer.Write(param.Type.ToString());
            writer.Write("</c> as represented in CLR type <c>");
            writer.Write(GetStringForType(param.Type, settings, true));
            writer.WriteLine("</c>.</param>");
        }

        // Remarks
        writer.WriteLine("/// <remarks>");

        WriteRemarkEntry("Introduced in", commandInfo.Build, writer);
        WriteRemarkEntry("PC hash", context.Hash, writer);
        if (!string.IsNullOrWhiteSpace(commandInfo.JenkinsHash))
        {
            WriteRemarkEntry("Original hash", commandInfo.JenkinsHash, writer);
        }

        writer.WriteLine("/// </remarks>");

        // Returns
        // Only write when it indeed has a return value.
        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            writer.Write("/// <returns>An instance of <c>");
            writer.Write(commandInfo.ReturnType.ToString());
            writer.Write("</c> as represented in CLR type <c>");
            writer.Write(context.ReturnTypeString);
            writer.WriteLine("</c>.</returns>");
        }
    }
}