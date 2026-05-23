// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation.Hooks;

/// <summary>
/// Implements method shim generation for Script Hook V .NET.
/// </summary>
internal sealed partial class VDotNetGenerator(GeneratorSettings settings) : IShimGenerator
{
    private const string CommonFieldHeader = "NWG_";
    private const string HashValueFieldFooter = "_Value";
    private const string ShimVariableFooter = "_shim";

    private const string ReturnValueVariable = "NWG_return_value";

    private string GetStringForType(ScriptCommandParameterType paramType, bool stripRef = false)
    {
        while (true)
        {
            // ReSharper disable once InvertIf
            if (stripRef && ParamUtil.PointerToRegularMap.TryGetValue(paramType,
                    out var resultType))
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

    private void WriteHashDefinition(in WrapperEmitContext context, TextWriter writer)
    {
        writer.Write("private static readonly global::GTA.Native.Hash ");
        writer.WriteSurround(CommonFieldHeader, context.SymbolNameHash,
            HashValueFieldFooter);
        writer.Write(" = (global::GTA.Native.Hash)");
        writer.Write(context.Hash);
        writer.WriteLine(';');
    }

    private void WriteMethodSignature(in WrapperEmitContext context,
        TextWriter writer)
    {
        var commandInfo = context.CommandInfo;

        // Write beginning
        // '<access> static <return-type> <name-or-hash>' and begin param list
        writer.Write(settings.Accessibility);
        writer.Write(" static ");
        writer.Write(context.ReturnTypeString);
        writer.Write(' ');
        writer.Write(commandInfo.Name != null
            ? MethodNameConverter.SnakeToPascal(commandInfo.Name)
            : context.SymbolNameHash);
        writer.Write('(');

        // Write parameters
        var afterFirst = false;
        foreach (var parameter in commandInfo.Parameters)
        {
            if (!afterFirst)
            {
                afterFirst = true;
            }
            else
            {
                writer.Write(',');
                writer.Write(' ');
            }

            writer.Write(GetStringForType(parameter.Type));
            writer.Write(' ');
            writer.WriteEscapedName(parameter.Name);
        }

        writer.Write(')');
    }

    private void WriteWrapperBodyNoPointer(in WrapperEmitContext context,
        TextWriter writer)
    {
        var commandInfo = context.CommandInfo;

        writer.WriteLine("{");

        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            writer.Write("return ");
        }

        writer.Write("global::GTA.Native.Function.Call");

        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            writer.Write('<');
            writer.Write(context.ReturnTypeString);
            writer.Write(">");
        }

        writer.Write('(');

        writer.WriteSurround(CommonFieldHeader, context.SymbolNameHash,
            HashValueFieldFooter);

        foreach (var param in commandInfo.Parameters)
        {
            writer.Write(',');
            writer.Write(' ');
            writer.WriteEscapedName(param.Name);
        }

        writer.WriteLine(");");
        writer.WriteLine('}');
    }

    public void WriteMethod(in WrapperEmitContext context, TextWriter writer)
    {
        WriteHashDefinition(context, writer);
        DocGenerator.WriteDocumentation(context, writer, settings);
        WriteMethodSignature(context, writer);

        if (ParamUtil.HasPointerParameter(context.CommandInfo.Parameters))
        {
            WriteWrapperBodyWithPointer(context, writer);
        }
        else
        {
            WriteWrapperBodyNoPointer(context, writer);
        }
    }
}