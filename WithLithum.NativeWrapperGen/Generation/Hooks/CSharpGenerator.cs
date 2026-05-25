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

            return Settings.ParameterTypes.TryGetValue(paramType, out var writeType)
                ? writeType
                : paramType.ToString();
        }
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

            writer.Write(GetStringForType(parameter.Type));
            writer.Write(' ');
            writer.WriteEscapedName(parameter.Name);
        }

        writer.Write(')');
    }

    public abstract void WriteMethod(in WrapperEmitContext context, TextWriter writer);
}
