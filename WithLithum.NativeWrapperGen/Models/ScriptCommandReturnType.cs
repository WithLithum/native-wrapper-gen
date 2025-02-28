namespace WithLithum.NativeWrapperGen.Models;

using System.Text.Json.Serialization;
using WithLithum.NativeWrapperGen.Serialization;

[JsonConverter(typeof(CommandReturnTypeConverter))]
public enum ScriptCommandReturnType
{
    Any,
    AnyPointer,
    Void,
    Int,
    Float,
    Boolean,
    String,
    Blip,
    Cam,
    Entity,
    FireId,
    Hash,
    Interior,
    ItemSet,
    Object,
    Ped,
    Pickup,
    Player,
    ScrHandle,
    Vector3,
    Vehicle
}
