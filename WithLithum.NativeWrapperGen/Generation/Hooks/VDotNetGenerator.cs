// SPDX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation.Hooks;

/// <summary>
/// Implements method shim generation for Script Hook V .NET.
/// </summary>
public sealed partial class VDotNetGenerator : CSharpGenerator
{
    public const string Id = "shvdn";
    public static readonly GeneratorSettings DefaultSettings = new()
    {
        TypeSettings = new()
        {
            AnyParameterType = "global::GTA.Native.InputArgument",
            AnyPointerType = "global::System.IntPtr",
            AnyReturnType = "object",
            HandleType = "int",
            HashType = "int",
            Vector3Type = "global::GTA.Math.Vector3",
            PlayerIdType = "int"
        },
        Accessibility = "public"
    };

    private const string HashValueFieldFooter = "_Value";

    public VDotNetGenerator(GeneratorSettings settings) : base(settings)
    {
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

        if (commandInfo.Parameters.Count > 0)
        {
            foreach (var param in commandInfo.Parameters)
            {
                writer.Write(", ");
                writer.WriteEscapedName(param.Name);
            }
        }
        else
        {
            writer.Write(',');
            WriteEmptyArray(writer);
        }

        writer.WriteLine(");");
        writer.WriteLine('}');
    }

    public override void WriteMethod(in WrapperEmitContext context, TextWriter writer)
    {
        WriteHashDefinition(context, writer);
        DocGenerator.WriteDocumentation(context, writer, Settings);
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
