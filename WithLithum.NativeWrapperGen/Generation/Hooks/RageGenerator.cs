// SPDX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Models;

namespace WithLithum.NativeWrapperGen.Generation.Hooks;

public sealed partial class RageGenerator : CSharpGenerator
{
    public const string Id = "rph";
    public static readonly GeneratorSettings DefaultSettings = new()
    {
        TypeSettings = new()
        {
            AnyParameterType = "global::Rage.Native.NativeArgument",
            AnyPointerType = "global::System.IntPtr",
            AnyReturnType = "int",
            HandleType = "uint",
            HashType = "uint",
            Vector3Type = "global::Rage.Vector3",
            PlayerIdType = "int"
        },
        Accessibility = "public"
    };

    private const string NativeCallMethod = "global::Rage.Native.NativeFunction.CallByHash";

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
            writer.WriteSurround(CommonFieldHeader, param.Name, ShimVariableFooter);
            writer.WriteLine(" = new global::Rage.Native.NativePointer();");

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
            writer.Write(", ");

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


    private void WriteDereference(TextWriter writer, IReadOnlyList<ScriptCommandParameterInfo> parameters)
    {
        foreach (var param in parameters
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
    }

    private void WriteWrapperBodyWithPointer(in WrapperEmitContext context,
        TextWriter writer)
    {
        var commandInfo = context.CommandInfo;
        var returnNotVoid = commandInfo.ReturnType != ScriptCommandReturnType.Void;
        var isComplex = commandInfo.ReturnType == ScriptCommandReturnType.String;

        // Per RPH documentation: write int for void return types.
        var returnTypeToken = returnNotVoid
                ? context.ReturnTypeString
                : "int";

        if (writer == null)
        {
            throw new InvalidOperationException("Writer not yet initialized.");
        }

        writer.WriteLine("{");

        // Generate ref shim variables
        WriteParameterShims(commandInfo.Parameters, writer);

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

        if (isComplex)
        {
            // Write explicit cast
            writer.Write('(');
            writer.Write(returnTypeToken);
            writer.Write(')');
        }

        writer.Write(NativeCallMethod);

        // Write normal syntax for value types.
        if (!isComplex)
        {
            writer.Write('<');
            writer.Write(returnTypeToken);
            writer.Write(">");
        }

        // Start writing arguments. Native function call method begins with hash.
        writer.Write('(');
        writer.Write(context.Hash);

        if (isComplex)
        {
            writer.Write(',');
            WriteTypeOf(writer, returnTypeToken);
        }

        WriteNativeCallPointerArguments(context, writer);

        writer.WriteLine(");"); // end arguments

        // Assign pointer values back to ref params
        var parameters = commandInfo.Parameters;
        WriteDereference(writer, parameters);

        // Return retVal
        if (context.CommandInfo.ReturnType != ScriptCommandReturnType.Void)
        {
            writer.WriteReturn(ReturnValueVariable);
        }

        writer.WriteLine('}'); // end block
    }

    #endregion

    #region Non-pointer body

    private void WriteWrapperBodyNoPointer(in WrapperEmitContext context,
        TextWriter writer)
    {
        var commandInfo = context.CommandInfo;
        var returnNotVoid = commandInfo.ReturnType != ScriptCommandReturnType.Void;

        // Per RPH documentation: write int for void return types.
        var returnTypeToken = returnNotVoid
                ? context.ReturnTypeString
                : "int";

        writer.WriteLine("{");
        writer.Write(returnNotVoid
            ? "return "
            : "_ = ");

        // If the return type is string (reference type) we will have to use the typeof syntax.
        //
        // Normal syntax: NativeFunction.CallByHash<ReturnType>(hash, params...)
        // Typeof syntax: (ReturnType)NativeFunction.CallByHash(hash, typeof(ReturnType), params...)
        var isComplex = commandInfo.ReturnType == ScriptCommandReturnType.String;

        if (isComplex)
        {
            // Write explicit cast
            writer.Write('(');
            writer.Write(returnTypeToken);
            writer.Write(')');
        }

        writer.Write(NativeCallMethod);

        // Write normal syntax for value types.
        if (!isComplex)
        {
            writer.Write('<');
            writer.Write(returnTypeToken);
            writer.Write(">");
        }

        // Start writing arguments. Native function call method begins with hash.
        writer.Write('(');
        writer.Write(context.Hash);

        if (isComplex)
        {
            writer.Write(',');
            WriteTypeOf(writer, returnTypeToken);
        }

        if (commandInfo.Parameters.Count > 0)
        {
            foreach (var param in commandInfo.Parameters)
            {
                writer.Write(',');
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

    #endregion
}
