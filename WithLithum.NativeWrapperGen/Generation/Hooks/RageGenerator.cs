// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation.Hooks;

public sealed partial class RageGenerator : CSharpGenerator
{
    public RageGenerator(GeneratorSettings settings) : base(settings)
    {
    }

    public override void WriteMethod(in WrapperEmitContext context, TextWriter writer)
    {
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

    #region Pointer body

    private static void WriteParameterShims(IReadOnlyList<ScriptCommandParameterInfo> paramList,
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

            writer.Write("global::Rage.Native.NativePointer ");
            //writer.Write(GetStringForType(param.Type, stripRef: true));
            //writer.Write(' ');
            writer.WriteSurround(CommonFieldHeader, param.Name, ShimVariableFooter);
            writer.Write(" = ");
            writer.WriteEscapedName(param.Name);
            writer.WriteLine(';');

            // Set native pointer value.
            writer.WriteSurround(CommonFieldHeader, param.Name, ShimVariableFooter);
            writer.Write(".SetValue(");
            writer.WriteEscapedName(param.Name);
            writer.WriteLine(");");
        }
    }

    // Use 'for' loop for speed.
    // ReSharper disable once ForCanBeConvertedToForeach
    private static void WriteNativeCallPointerArguments(in WrapperEmitContext context,
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

        // Store return value in variable
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            writer.Write("{0} = ", ReturnValueVariable);
        }

        writer.Write("global::Rage.Native.NativeFunction.Call");

        // Return type
        writer.Write('<');

        // Per https://docs.ragepluginhook.net/html/M_Rage_Native_NativeFunction_CallByHash__1.htm
        // "If the native doesn't return a value pass int."
        writer.Write(context.CommandInfo.ReturnType == ScriptCommandReturnType.Void
            ? "int"
            : context.ReturnTypeString);

        writer.Write('>');

        // Arguments
        writer.Write('(');
        writer.Write(context.Hash);

        WriteNativeCallPointerArguments(context, writer);

        writer.WriteLine(");"); // end arguments

        // Assign pointer values back to ref params
        foreach (var param in context.CommandInfo.Parameters
                     .Where(static x => ParamUtil.IsPointerType(x.Type)))
        {
            writer.WriteEscapedName(param.Name);
            writer.Write(" = ");
            writer.WriteSurround(CommonFieldHeader, param.Name, ShimVariableFooter);
            writer.Write(".GetValue<");
            writer.Write(GetStringForType(ParamUtil.PointerToRegularMap[param.Type]));
            writer.Write('>');
            writer.WriteLine("();");

            // We need to release the native pointer because it is a disposable.
            //
            // Not going to write a 'finally' block because if something fails here the most likely
            // outcome is that the game CTDs.
            writer.WriteSurround(CommonFieldHeader, param.Name, ShimVariableFooter);
            writer.Write(".Dispose();");
            writer.WriteLine();
        }

        // Return retVal
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            writer.WriteReturn(ReturnValueVariable);
        }

        writer.WriteLine('}'); // end block
    }

    #endregion

    #region Non-pointer body

    private static void WriteWrapperBodyNoPointer(in WrapperEmitContext context,
        TextWriter writer)
    {
        var commandInfo = context.CommandInfo;

        writer.WriteLine("{");
        writer.Write(commandInfo.ReturnType != ScriptCommandReturnType.Void
            ? "return "
            : "_ = ");

        writer.Write("global::Rage.Native.NativeFunction.CallByHash");

        // Return type!
        writer.Write('<');
        writer.Write(commandInfo.ReturnType != ScriptCommandReturnType.Void
            ? context.ReturnTypeString
            : "int");
        writer.Write(">");

        // Call
        writer.Write('(');
        writer.Write(context.Hash);

        foreach (var param in commandInfo.Parameters)
        {
            writer.Write(',');
            writer.Write(' ');
            writer.WriteEscapedName(param.Name);
        }

        writer.WriteLine(");");
        writer.WriteLine('}');
    }

    #endregion
}
