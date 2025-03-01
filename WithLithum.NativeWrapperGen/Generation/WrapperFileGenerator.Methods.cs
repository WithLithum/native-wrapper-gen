namespace WithLithum.NativeWrapperGen.Generation;

using WithLithum.NativeWrapperGen.Models;

public partial class WrapperFileGenerator
{
    private const string HashValueFieldTemplate = "NWG_{0}_Value";
    private const string ShimVariableTemplate = "NWG_{0}_shim";

    internal void WriteDocumentation(string hash, ScriptCommandInfo commandInfo)
    {
        // Summary
        if (!string.IsNullOrWhiteSpace(commandInfo.Comment))
        {
            _writer.WriteLine("/// <summary>");

            _writer.Write("/// ");
            _writer.WriteLine(commandInfo.Comment.Replace("\n", "<br />"));

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
            _writer.Write(GetStringForType(commandInfo.ReturnType));
            _writer.WriteLine("</c>.</param>");
        }

        // Remarks
        _writer.WriteLine("/// <remarks>");

        _writer.Write("/// <b>Introduced in</b>: ");
        _writer.Write(commandInfo.Build);
        _writer.WriteLine("<br />");

        _writer.Write("/// <b>PC Hash</b>: ");
        _writer.Write(hash);
        _writer.WriteLine("<br />");

        _writer.WriteLine("/// </remarks>");

        // Returns
        _writer.Write("/// <returns>An instance of <c>");
        _writer.Write(commandInfo.ReturnType.ToString());
        _writer.Write("</c> as represented in CLR type <c>");
        _writer.Write(GetStringForType(commandInfo.ReturnType));
        _writer.WriteLine("</c>.</returns>");
    }

    internal void WriteHashDefinition(string hash)
    {
        _writer.Write("private static readonly global::GTA.Native.Hash ");
        _writer.Write(HashValueFieldTemplate, MethodNameConverter.HashToMethodName(hash));
        _writer.Write(" = (global::GTA.Native.Hash)");
        _writer.Write(hash);
        _writer.WriteLine(';');
    }

    internal void WriteWrapperBodyNoPointer(string hash, ScriptCommandInfo commandInfo)
    {
        var returnTypeName = GetStringForType(commandInfo.ReturnType);

        _writer.WriteLine("{");

        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write("return ");
        }

        _writer.Write("global::GTA.Native.Function.Call");

        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write('<');
            _writer.Write(returnTypeName);
            _writer.Write(">");
        }

        _writer.Write('(');

        _writer.Write(HashValueFieldTemplate, MethodNameConverter.HashToMethodName(hash));

        foreach (var param in commandInfo.Parameters)
        {
            _writer.Write(',');
            _writer.Write(' ');
            _writer.Write(ParamUtil.EscapeName(param.Name));
        }

        _writer.WriteLine(");");
        _writer.WriteLine('}');
    }

    internal void WriteWrapperBodyWithPointer(string hash, ScriptCommandInfo commandInfo)
    {
        var returnTypeName = GetStringForType(commandInfo.ReturnType);

        _writer.WriteLine("{");

        // Generate ref shim variables
        foreach (var param in commandInfo.Parameters
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
        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.WriteLine("{0} NWG_retval;", returnTypeName);
        }

        // Generate call body
        _writer.WriteLine("unsafe {");

        // Store return value in variable
        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write("NWG_retval = ");
        }

        _writer.Write("global::GTA.Native.Function.Call");

        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write('<');
            _writer.Write(returnTypeName);
            _writer.Write('>');
        }

        _writer.Write('('); // begin arguments
        _writer.Write(HashValueFieldTemplate, MethodNameConverter.HashToMethodName(hash));

        foreach (var param in commandInfo.Parameters)
        {
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
        foreach (var param in commandInfo.Parameters
            .Where(x => ParamUtil.IsPointerType(x.Type)))
        {
            _writer.Write(ParamUtil.EscapeName(param.Name));
            _writer.Write(" = ");
            _writer.Write(ShimVariableTemplate, param.Name);
            _writer.WriteLine(';');
        }

        // Return retVal
        if (commandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            _writer.Write("return NWG_retval;");
        }

        _writer.WriteLine('}'); // end block
    }

    internal void WriteMethodSignature(string hash, ScriptCommandInfo commandInfo)
    {
        // Write beginning
        _writer.Write(_settings.Accessibility);
        _writer.Write(" static ");
        _writer.Write(GetStringForType(commandInfo.ReturnType));
        _writer.Write(' ');
        _writer.Write(commandInfo.Name != null
            ? MethodNameConverter.SnakeToPascal(commandInfo.Name)
            : MethodNameConverter.HashToMethodName(hash));
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

    internal void WriteWrapperMethod(string hash, ScriptCommandInfo commandInfo)
    {
        var paramsHasPointer = commandInfo.Parameters.Any(x => ParamUtil.IsPointerType(x.Type));

        _writer.WriteLine();
        _writer.WriteLine("// ---------------------------------------------------");
        _writer.WriteLine("// {0}", commandInfo.Name ?? hash);
        _writer.WriteLine("// ---------------------------------------------------");
        _writer.WriteLine();

        WriteHashDefinition(hash);
        WriteDocumentation(hash, commandInfo);
        WriteMethodSignature(hash, commandInfo);
        
        if (paramsHasPointer)
        {
            WriteWrapperBodyWithPointer(hash, commandInfo);
        }
        else
        {
            WriteWrapperBodyNoPointer(hash, commandInfo);
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
