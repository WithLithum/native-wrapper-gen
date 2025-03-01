namespace WithLithum.NativeWrapperGen.Models;

using System.Text.Json.Serialization;
using WithLithum.NativeWrapperGen.Serialization;

[JsonConverter(typeof(CommandParameterTypeConverter))]
public enum ScriptCommandParameterType
{
    Any,
    AnyPointer,
    Void,
    Int,
    IntPointer,
    Float,
    FloatPointer,
    Boolean,
    BooleanPointer,
    MutableString,
    String,
    Blip,
    BlipPointer,
    Cam,
    CamPointer,
    Entity,
    EntityPointer,
    FireId,
    FireIdPointer,
    Hash,
    HashPointer,
    Interior,
    InteriorPointer,
    ItemSet,
    ItemSetPointer,
    Object,
    ObjectPointer,
    Ped,
    PedPointer,
    Pickup,
    PickupPointer,
    Player,
    PlayerPointer,
    ScrHandle,
    ScrHandlePointer,
    Vector3Pointer,
    Vehicle,
    VehiclePointer,
    Vector3
}
