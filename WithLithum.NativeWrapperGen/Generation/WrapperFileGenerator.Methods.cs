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
namespace WithLithum.NativeWrapperGen.Generation;

using System.Text;
using WithLithum.NativeWrapperGen.Models;

public partial class WrapperFileGenerator
{
    private const string HashValueFieldTemplate = "NWG_{0}_Value";
    private const string ShimVariableTemplate = "NWG_{0}_shim";

    private static string EscapeForDocumentation(string comment)
    {
        var sb = new StringBuilder(comment)
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\n", "<br />");

        return sb.ToString();
    }

    internal void WriteDocumentation(WrapperEmitContext context)
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

        _writer.Write("/// <b>Introduced in</b>: ");
        _writer.Write(commandInfo.Build);
        _writer.WriteLine("<br />");

        _writer.Write("/// <b>PC Hash</b>: ");
        _writer.Write(context.Hash);
        _writer.WriteLine("<br />");

        _writer.WriteLine("/// </remarks>");

        // Returns
        _writer.Write("/// <returns>An instance of <c>");
        _writer.Write(commandInfo.ReturnType.ToString());
        _writer.Write("</c> as represented in CLR type <c>");
        _writer.Write(context.ReturnTypeString);
        _writer.WriteLine("</c>.</returns>");
    }

    internal void WriteHashDefinition(WrapperEmitContext context)
    {
        _writer.Write("private static readonly global::GTA.Native.Hash ");
        _writer.Write(HashValueFieldTemplate, context.SymbolNameHash);
        _writer.Write(" = (global::GTA.Native.Hash)");
        _writer.Write(context.Hash);
        _writer.WriteLine(';');
    }

    internal void WriteWrapperBodyNoPointer(WrapperEmitContext context)
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

        _writer.Write(HashValueFieldTemplate, context.SymbolNameHash);

        foreach (var param in commandInfo.Parameters)
        {
            _writer.Write(',');
            _writer.Write(' ');
            _writer.Write(ParamUtil.EscapeName(param.Name));
        }

        _writer.WriteLine(");");
        _writer.WriteLine('}');
    }

    internal void WriteWrapperBodyWithPointer(WrapperEmitContext context)
    {
        _writer.WriteLine("{");

        // Generate ref shim variables
        foreach (var param in context.CommandInfo.Parameters
            .Where(x => ParamUtil.IsPointerType(x.Type)))
        {
            var shimName = string.Format(ShimVariableTemplate, param.Name);
            _writer.Write(GetStringForType(param.Type, stripRef: true));
            _writer.Write(' ');
            _writer.Write(shimName);
            _writer.Write(" = ");
            _writer.Write(ParamUtil.EscapeName(param.Name));
            _writer.WriteLine(';');
        }

        // Create return type variable if necessary
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.WriteLine("{0} NWG_retval;", context.ReturnTypeString);
        }

        // Generate call body
        _writer.WriteLine("unsafe {");

        // Store return value in variable
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write("NWG_retval = ");
        }

        _writer.Write("global::GTA.Native.Function.Call");

        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write('<');
            _writer.Write(context.ReturnTypeString);
            _writer.Write('>');
        }

        _writer.Write('('); // begin arguments
        _writer.Write(HashValueFieldTemplate, context.SymbolNameHash);

        // Use 'for' loop for speed.
        for (int i = 0; i < context.CommandInfo.Parameters.Count; i++)
        {
            ScriptCommandParameterInfo param = context.CommandInfo.Parameters[i];
            _writer.Write(',');
            _writer.Write(' ');
            if (ParamUtil.IsPointerType(param.Type))
            {
                _writer.Write('&');
                _writer.Write(ShimVariableTemplate, param.Name);
            }
            else
            {
                _writer.Write(ParamUtil.EscapeName(param.Name));
            }
        }

        _writer.WriteLine(");"); // end arguments
        _writer.WriteLine('}'); // end unsafe

        // Assign shims values back to their ref fields
        foreach (var param in context.CommandInfo.Parameters
            .Where(x => ParamUtil.IsPointerType(x.Type)))
        {
            _writer.Write(ParamUtil.EscapeName(param.Name));
            _writer.Write(" = ");
            _writer.Write(ShimVariableTemplate, param.Name);
            _writer.WriteLine(';');
        }

        // Return retVal
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write("return NWG_retval;");
        }

        _writer.WriteLine('}'); // end block
    }

    internal void WriteMethodSignature(WrapperEmitContext context)
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
            _writer.Write(ParamUtil.EscapeName(parameter.Name));
        }

        _writer.Write(')');
    }

    internal void WriteWrapperMethod(WrapperEmitContext context)
    {
        var commandInfo = context.CommandInfo;

        var paramsHasPointer = context.CommandInfo.Parameters.Any(x => ParamUtil.IsPointerType(x.Type));

        _writer.WriteLine();
        _writer.WriteLine("// ---------------------------------------------------");
        _writer.WriteLine("// {0}", commandInfo.Name ?? context.Hash);
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

    internal string GetStringForType(ScriptCommandParameterType paramType,
        bool stripRef = false)
    {
        if (stripRef
            && ParamUtil.PointerToRegularMap.TryGetValue(paramType, out var resultType))
        {
            return GetStringForType(resultType, false);
        }

        if (_settings.ParameterTypes.TryGetValue(paramType, out var writeType))
        {
            return writeType;
        }

        return paramType.ToString();
    }

    internal string GetStringForType(ScriptCommandReturnType returnType)
    {
        if (_settings.ReturnTypes.TryGetValue(returnType, out var writeType))
        {
            return writeType;
        }

        return returnType.ToString();
    }
}
