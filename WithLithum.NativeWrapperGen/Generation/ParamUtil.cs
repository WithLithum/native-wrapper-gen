namespace WithLithum.NativeWrapperGen.Generation;

using WithLithum.NativeWrapperGen.Models;

internal static class ParamUtil
{
    private static readonly HashSet<ScriptCommandParameterType> PointerTypes =
    [
        ScriptCommandParameterType.FireIdPointer,
        ScriptCommandParameterType.IntPointer,
        ScriptCommandParameterType.PlayerPointer,
        ScriptCommandParameterType.PickupPointer,
        ScriptCommandParameterType.PedPointer,
        ScriptCommandParameterType.AnyPointer,
        ScriptCommandParameterType.BooleanPointer,
        ScriptCommandParameterType.CamPointer,
        ScriptCommandParameterType.EntityPointer,
        ScriptCommandParameterType.VehiclePointer,
        ScriptCommandParameterType.ScrHandlePointer,
        ScriptCommandParameterType.FloatPointer,
        ScriptCommandParameterType.InteriorPointer,
        ScriptCommandParameterType.BlipPointer,
        ScriptCommandParameterType.Vector3Pointer,
        ScriptCommandParameterType.ItemSetPointer,
        ScriptCommandParameterType.HashPointer,
        ScriptCommandParameterType.ObjectPointer
    ];

    private static readonly HashSet<string> EscapeParamNames =
    [
        "object",
        "string",
        "event",
        "override",
        "in",
        "out",
        "base"
    ];

    internal static string EscapeName(string name)
    {
        if (EscapeParamNames.Contains(name))
        {
            return $"@{name}";
        }

        return name;
    }

    internal static bool IsPointerType(ScriptCommandParameterType type)
    {
        return PointerTypes.Contains(type);
    }

    internal static string StripRef(string typeName)
    {
        // TODO: This is temporary measure
        if (!typeName.StartsWith("ref"))
        {
            return typeName;
        }

        return typeName[4..];
    }
}
