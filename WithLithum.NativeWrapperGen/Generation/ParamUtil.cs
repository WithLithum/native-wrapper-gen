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

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using SCPT = Models.ScriptCommandParameterType;

internal static class ParamUtil
{
    internal static readonly ReadOnlyDictionary<SCPT, SCPT> PointerToRegularMap =
    new Dictionary<SCPT, SCPT>
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteReturn(this TextWriter writer,
        string context)
    {
        writer.Write("return ");
        writer.Write(context);
        writer.Write(';');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteSurround(this TextWriter writer,
        string head,
        string content,
        string foot)
    {
        writer.Write(head);
        writer.Write(content);
        writer.Write(foot);
    }

    internal static void WriteEscapedName(this TextWriter writer, string name)
    {
        writer.Write('@');
        writer.Write(name);
    }

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
