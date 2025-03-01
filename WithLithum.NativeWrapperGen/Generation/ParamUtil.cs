namespace WithLithum.NativeWrapperGen.Generation;

using System.Collections.ObjectModel;

using SCPT = Models.ScriptCommandParameterType;

internal static class ParamUtil
{
    internal static readonly ReadOnlyDictionary<SCPT, SCPT> PointerToRegularMap =
    new Dictionary<SCPT, SCPT>()
    {
        { SCPT.FireIdPointer, SCPT.FireId },
        { SCPT.IntPointer, SCPT.Int },
        { SCPT.PlayerPointer, SCPT.Player },
        { SCPT.PickupPointer, SCPT.Pickup },
        { SCPT.PedPointer, SCPT.Ped },
        { SCPT.BooleanPointer, SCPT.Boolean },
        { SCPT.CamPointer, SCPT.Cam },
        { SCPT.EntityPointer, SCPT.Entity },
        { SCPT.VehiclePointer, SCPT.Vehicle },
        { SCPT.ScrHandlePointer, SCPT.ScrHandle },
        { SCPT.FloatPointer, SCPT.Float },
        { SCPT.InteriorPointer, SCPT.Interior },
        { SCPT.BlipPointer, SCPT.Blip },
        { SCPT.Vector3Pointer, SCPT.Vector3 },
        { SCPT.ItemSetPointer, SCPT.ItemSet },
        { SCPT.HashPointer, SCPT.Hash },
        { SCPT.ObjectPointer, SCPT.Object }
    }.AsReadOnly();

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

    internal static bool IsPointerType(SCPT type)
    {
        return PointerToRegularMap.ContainsKey(type);
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
