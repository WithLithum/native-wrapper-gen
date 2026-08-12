using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation.Hooks;

public partial class VDotNetGenerator
{
    private void WriteParameterShims(IReadOnlyList<ScriptCommandParameterInfo> paramList,
        TextWriter writer)
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

            writer.Write(GetStringForType(param.Type, stripRef: true));
            writer.Write(' ');
            writer.WriteSurround(CommonFieldHeader, param.Name, ShimVariableFooter);
            writer.Write(" = ");
            writer.WriteEscapedName(param.Name);
            writer.WriteLine(';');
        }
    }

    // Use 'for' loop for speed.
    // ReSharper disable once ForCanBeConvertedToForeach
    private void WriteNativeCallPointerArguments(in WrapperEmitContext context,
        TextWriter writer)
    {
        var paramList = context.CommandInfo.Parameters;
        for (var i = 0; i < paramList.Count; i++)
        {
            var param = paramList[i];
            writer.Write(',');
            writer.Write(' ');
            if (ParamUtil.IsPointerType(param.Type))
            {
                writer.Write('&');
                writer.WriteSurround(CommonFieldHeader, param.Name, ShimVariableFooter);
            }
            else
            {
                writer.WriteEscapedName(param.Name);
            }
        }
    }

    private void WriteWrapperBodyWithPointer(in WrapperEmitContext context,
        TextWriter writer)
    {
        if (writer == null)
        {
            throw new InvalidOperationException("Writer not yet initialized.");
        }

        writer.WriteLine("{");

        // Generate ref shim variables
        WriteParameterShims(context.CommandInfo.Parameters, writer);

        // Create return type variable if necessary
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            writer.Write(context.ReturnTypeString);
            writer.Write(' ');
            writer.Write(ReturnValueVariable);
            writer.WriteLine(';');
        }

        // Generate call body
        writer.WriteLine("unsafe {");

        // Store return value in variable
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            writer.Write("{0} = ", ReturnValueVariable);
        }

        writer.Write("global::GTA.Native.Function.Call");

        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            writer.Write('<');
            writer.Write(context.ReturnTypeString);
            writer.Write('>');
        }

        writer.Write('('); // begin arguments
        
        writer.Write(CommonFieldHeader);
        WriteHashMethodName(context, writer);
        writer.Write(HashValueFieldFooter);
        
        WriteNativeCallPointerArguments(context, writer);

        writer.WriteLine(");"); // end arguments
        writer.WriteLine('}'); // end unsafe

        // Assign shims values back to their ref fields
        foreach (var paramName in context.CommandInfo.Parameters
                     .Where(static x => ParamUtil.IsPointerType(x.Type))
                     .Select(x => x.Name))
        {
            writer.WriteEscapedName(paramName);
            writer.Write(" = ");
            writer.WriteSurround(CommonFieldHeader, paramName, ShimVariableFooter);
            writer.WriteLine(';');
        }

        // Return retVal
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            writer.WriteReturn(ReturnValueVariable);
        }

        writer.WriteLine('}'); // end block
    }
}
