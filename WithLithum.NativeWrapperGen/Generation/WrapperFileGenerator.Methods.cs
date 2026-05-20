// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using System.Security;
using System.Text;
using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation;

public partial class WrapperFileGenerator
{
    private const string CommonFieldHeader = "NWG_";
    private const string HashValueFieldFooter = "_Value";
    private const string ShimVariableFooter = "_shim";

    private const string ReturnValueVariable = "NWG_return_value";

    private static string EscapeForDocumentation(string comment)
    {
        var sb = new StringBuilder(comment)
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\n", "<br />");

        return sb.ToString();
    }

    private void WriteRemarkEntry(string property, string value)
    {
        // Property name
        _writer.Write("/// <b>");
        _writer.Write(SecurityElement.Escape(property));
        _writer.Write("</b>: ");

        _writer.Write(SecurityElement.Escape(value));
        _writer.WriteLine("<br />");
    }

    private void WriteDocumentation(WrapperEmitContext context)
    {
        var commandInfo = context.CommandInfo;

        // Summary
        if (!string.IsNullOrWhiteSpace(commandInfo.Comment))
        {
            _writer.WriteLine("/// <summary>");

            _writer.Write("/// ");
            _writer.WriteLine(EscapeForDocumentation(commandInfo.Comment));

            _writer.WriteLine("/// </summary>");
        }

        // Parameters
        foreach (var param in commandInfo.Parameters)
        {
            _writer.Write("/// <param name=\"");
            _writer.Write(param.Name);
            _writer.Write("\">An instance of <c>");
            _writer.Write(param.Type.ToString());
            _writer.Write("</c> as represented in CLR type <c>");
            _writer.Write(GetStringForType(param.Type, true));
            _writer.WriteLine("</c>.</param>");
        }

        // Remarks
        _writer.WriteLine("/// <remarks>");

        WriteRemarkEntry("Introduced in", commandInfo.Build);
        WriteRemarkEntry("PC hash", context.Hash);
        if (!string.IsNullOrWhiteSpace(commandInfo.JenkinsHash))
        {
            WriteRemarkEntry("Original hash", commandInfo.JenkinsHash);
        }

        _writer.WriteLine("/// </remarks>");

        // Returns
        // Only write when it indeed has a return value.
        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write("/// <returns>An instance of <c>");
            _writer.Write(commandInfo.ReturnType.ToString());
            _writer.Write("</c> as represented in CLR type <c>");
            _writer.Write(context.ReturnTypeString);
            _writer.WriteLine("</c>.</returns>");
        }
    }

    private void WriteHashDefinition(WrapperEmitContext context)
    {
        _writer.Write("private static readonly global::GTA.Native.Hash ");
        _writer.WriteSurround(CommonFieldHeader, context.SymbolNameHash, HashValueFieldFooter);
        _writer.Write(" = (global::GTA.Native.Hash)");
        _writer.Write(context.Hash);
        _writer.WriteLine(';');
    }

    private void WriteWrapperBodyNoPointer(WrapperEmitContext context)
    {
        var commandInfo = context.CommandInfo;

        _writer.WriteLine("{");

        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write("return ");
        }

        _writer.Write("global::GTA.Native.Function.Call");

        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write('<');
            _writer.Write(context.ReturnTypeString);
            _writer.Write(">");
        }

        _writer.Write('(');

        _writer.WriteSurround(CommonFieldHeader, context.SymbolNameHash, HashValueFieldFooter);

        foreach (var param in commandInfo.Parameters)
        {
            _writer.Write(',');
            _writer.Write(' ');
            _writer.WriteEscapedName(param.Name);
        }

        _writer.WriteLine(");");
        _writer.WriteLine('}');
    }

    private void WriteWrapperBodyWithPointer(WrapperEmitContext context)
    {
        if (_writer == null)
        {
            throw new InvalidOperationException("Writer not yet initialized.");
        }

        _writer.WriteLine("{");

        // Generate ref shim variables
        WriteParameterShims(context.CommandInfo.Parameters);

        // Create return type variable if necessary
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write(context.ReturnTypeString);
            _writer.Write(' ');
            _writer.Write(ReturnValueVariable);
        }

        // Generate call body
        _writer.WriteLine("unsafe {");

        // Store return value in variable
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write("{0} = ", ReturnValueVariable);
        }

        _writer.Write("global::GTA.Native.Function.Call");

        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write('<');
            _writer.Write(context.ReturnTypeString);
            _writer.Write('>');
        }

        _writer.Write('('); // begin arguments
        _writer.WriteSurround(CommonFieldHeader, context.SymbolNameHash, HashValueFieldFooter);

        WriteNativeCallPointerArguments(context);

        _writer.WriteLine(");"); // end arguments
        _writer.WriteLine('}'); // end unsafe

        // Assign shims values back to their ref fields
        foreach (var paramName in context.CommandInfo.Parameters
            .Where(static x => ParamUtil.IsPointerType(x.Type))
            .Select(x => x.Name))
        {
            _writer.WriteEscapedName(paramName);
            _writer.Write(" = ");
            _writer.WriteSurround(CommonFieldHeader, paramName, ShimVariableFooter);
            _writer.WriteLine(';');
        }

        // Return retVal
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.WriteReturn(ReturnValueVariable);
        }

        _writer.WriteLine('}'); // end block
    }

    private void WriteMethodSignature(WrapperEmitContext context)
    {
        var commandInfo = context.CommandInfo;

        // Write beginning
        _writer.Write(_settings.Accessibility);
        _writer.Write(" static ");
        _writer.Write(context.ReturnTypeString);
        _writer.Write(' ');
        _writer.Write(commandInfo.Name != null
            ? MethodNameConverter.SnakeToPascal(commandInfo.Name)
            : context.SymbolNameHash);
        _writer.Write('(');

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
                _writer.Write(',');
                _writer.Write(' ');
            }

            _writer.Write(GetStringForType(parameter.Type));
            _writer.Write(' ');
            _writer.WriteEscapedName(parameter.Name);
        }

        _writer.Write(')');
    }

    private void WriteParameterShims(IReadOnlyList<ScriptCommandParameterInfo> paramList)
    {
        // Use 'for' loop for speed.
        // ReSharper disable once ForCanBeConvertedToForeach
        for (int i = 0; i < paramList.Count; i++)
        {
            var param = paramList[i];
            if (!ParamUtil.IsPointerType(param.Type))
            {
                continue;
            }

            _writer.Write(GetStringForType(param.Type, stripRef: true));
            _writer.Write(' ');
            _writer.WriteSurround(CommonFieldHeader, param.Name, ShimVariableFooter);
            _writer.Write(" = ");
            _writer.WriteEscapedName(param.Name);
            _writer.WriteLine(';');
        }
    }

    // Use 'for' loop for speed.
    // ReSharper disable once ForCanBeConvertedToForeach
    private void WriteNativeCallPointerArguments(WrapperEmitContext context)
    {
        var paramList = context.CommandInfo.Parameters;
        for (var i = 0; i < paramList.Count; i++)
        {
            var param = paramList[i];
            _writer.Write(',');
            _writer.Write(' ');
            if (ParamUtil.IsPointerType(param.Type))
            {
                _writer.Write('&');
                _writer.WriteSurround(CommonFieldHeader, param.Name, ShimVariableFooter);
            }
            else
            {
                _writer.WriteEscapedName(param.Name);
            }
        }
    }

    private void WriteWrapperMethod(WrapperEmitContext context)
    {
        var commandInfo = context.CommandInfo;

        var paramsHasPointer = context.CommandInfo.Parameters.Any(x => ParamUtil.IsPointerType(x.Type));

        _writer.WriteLine();
        _writer.WriteLine("// ---------------------------------------------------");
        _writer.Write("// ");
        _writer.WriteLine(commandInfo.Name ?? context.Hash);
        _writer.WriteLine("// ---------------------------------------------------");
        _writer.WriteLine();

        WriteHashDefinition(context);
        WriteDocumentation(context);
        WriteMethodSignature(context);

        if (paramsHasPointer)
        {
            WriteWrapperBodyWithPointer(context);
        }
        else
        {
            WriteWrapperBodyNoPointer(context);
        }
    }

    private string GetStringForType(ScriptCommandParameterType paramType, bool stripRef = false)
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

            return _settings.ParameterTypes.TryGetValue(paramType, out var writeType)
                ? writeType
                : paramType.ToString();
        }
    }

    private string GetStringForType(ScriptCommandReturnType returnType)
    {
        return _settings.ReturnTypes.TryGetValue(returnType, out var writeType)
            ? writeType
            : returnType.ToString();
    }
}
