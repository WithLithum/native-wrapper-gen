// SPDX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation.Hooks;

public abstract class CSharpGenerator : IShimGenerator
{
    public const string CommonFieldHeader = "NWG_";
    public const string ShimVariableFooter = "_shim";
    public const string ReturnValueVariable = "NWG_return_value";

    public GeneratorSettings Settings { get; }

    protected CSharpGenerator(GeneratorSettings settings)
    {
        Settings = settings;
    }

    protected string GetStringForType(ScriptCommandParameterType paramType, bool stripRef = false)
    {
        if (ParamUtil.PointerToRegularMap.TryGetValue(paramType,
                out var resultType))
        {
            var resultTypeName = Settings.TypeSettings.GetTypeName(resultType);
            return stripRef
                ? resultTypeName
                : $"ref {resultTypeName}";
        }

        return Settings.TypeSettings.GetTypeName(paramType);
    }

    private void WriteParameterInternalByRef(ScriptCommandParameterType parameterType,
        TextWriter writer)
    {
        if (!ParamUtil.PointerToRegularMap.TryGetValue(parameterType, out var regularType))
        {
            throw new ArgumentException("The specified type is not a pointer type.",
                nameof(parameterType));
        }

        writer.Write("ref ");
        writer.Write(Settings.TypeSettings.GetTypeName(regularType));
    }

    protected void WriteEmptyArray(TextWriter writer, string? type = null)
    {
        type ??= Settings.TypeSettings.AnyParameterType;

        writer.Write("global::System.Array.Empty<");
        writer.Write(type);
        writer.Write(">()");
    }

    protected static void WriteTypeOf(TextWriter writer, string type)
    {
        writer.Write("typeof(");
        writer.Write(type);
        writer.Write(')');
    }

    protected void WriteParameter(ScriptCommandParameterInfo parameterInfo,
        TextWriter writer)
    {
        // AnyPointer is to be written as IntPtr
        if (parameterInfo.Type != ScriptCommandParameterType.AnyPointer
            && ParamUtil.IsPointerType(parameterInfo.Type))
        {
            WriteParameterInternalByRef(parameterInfo.Type, writer);
        }
        else
        {
            writer.Write(Settings.TypeSettings.GetTypeName(parameterInfo.Type));
        }

        writer.Write(' ');
        writer.WriteEscapedName(parameterInfo.Name);
    }

    protected internal void WriteMethodSignature(in WrapperEmitContext context,
        TextWriter writer)
    {
        var commandInfo = context.CommandInfo;

        // Write beginning
        // '<access> static <return-type> <name-or-hash>' and begin param list
        writer.Write(Settings.Accessibility);
        writer.Write(" static ");
        writer.Write(context.ReturnTypeString);
        writer.Write(' ');

        if (commandInfo.Name != null)
        {
            if (!string.IsNullOrWhiteSpace(commandInfo.SymbolName))
            {
                writer.Write(commandInfo.SymbolName);
            }
            else
            {
                WriteSnakeToPascal(commandInfo.Name, writer);   
            }
        }
        else
        {
            writer.Write(context.SymbolNameHash);
        }
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

            WriteParameter(parameter, writer);
        }

        writer.Write(')');
    }

    protected void WriteSnakeToPascal(string input,
        TextWriter writer)
    {
        if (input.Length < MethodNameConverter.MaximumCharacterLength)
        {
            Span<char> nameBuf = stackalloc char[input.Length];
            var written = MethodNameConverter.SnakeToPascal(input.AsSpan(),
                nameBuf);
            writer.Write(nameBuf[..written]);
        }
        else
        {
            writer.Write(MethodNameConverter.SnakeToPascal(input));
        }
    }

    public abstract void WriteMethod(in WrapperEmitContext context, TextWriter writer);
}
