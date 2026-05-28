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
        writer.Write(Settings.TypeSettings.GetTypeName(parameterType));
    }

    protected void WriteParameter(ScriptCommandParameterInfo parameterInfo,
        TextWriter writer)
    {
        if (ParamUtil.IsPointerType(parameterInfo.Type))
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

    protected void WriteMethodSignature(in WrapperEmitContext context,
        TextWriter writer)
    {
        var commandInfo = context.CommandInfo;

        // Write beginning
        // '<access> static <return-type> <name-or-hash>' and begin param list
        writer.Write(Settings.Accessibility);
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

            WriteParameter(parameter, writer);
        }

        writer.Write(')');
    }

    public abstract void WriteMethod(in WrapperEmitContext context, TextWriter writer);
}
