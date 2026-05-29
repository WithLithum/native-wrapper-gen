# Version migration guide

## 0.3.0

### Configuration files

In 0.3.0, `parameter_types` and `return_types` fields have been consolidated into the
`type_settings` property, and is now primarily controlled by 7 properties instead of being
individually mapped.

This won't affect you if you are using a default configuration. If you uses a custom configuration
file, the file will need to be updated.

A configuration file after the consolidation looks like this:

```json
{
  "type_settings": {
    "any_parameter_type": "NativeArgument",
    "any_pointer_type": "NativePointer",
    "any_return_type": "int",
    "handle_type": "uint",
    "hash_type": "uint",
    "vector3_type": "Vector3",
    "player_id_type": "int",
    "return_type_overrides": {},
    "parameter_type_overrides": {}
  },
  "accessibility": "public"
}
```

Here is an explanation of the new `type_settings` properties:

- `any_parameter_type`: The type used for an `Any` parameter
- `any_pointer_type`: The type corresponding to a pointer of `Any`
- `any_return_type`: The type used for `Any` return values
- `handle_type`: The type used for pool handles (entities, peds, vehicles, etc.)
- `hash_type`: The type used for JOAAT hashes (models, etc.)
- `vector3_type`: The type used for `Vector3`. This should be your hook's `Vector3`.
- `player_id_type`: The type used for player ID.

You may also define overrides for parameter types in `parameter_type_overrides` or for return types
in `return_type_overrides`.

#### Samples

The following are the JSON form of the SHVDN defaults:

```json
{
  "type_settings": {
    "any_parameter_type": "global::GTA.Native.InputArgument",
    "any_pointer_type": "global::System.IntPtr",
    "any_return_type": "object",
    "handle_type": "int",
    "hash_type": "int",
    "vector3_type": "global::GTA.Math.Vector3",
    "player_id_type": "int"
  },
  "accessibility": "public"
}
```

and the following are RPH defaults:

```json
{
  "type_settings": {
    "any_parameter_type": "global::Rage.Native.NativeArgument",
    "any_pointer_type": "global::System.IntPtr",
    "any_return_type": "int",
    "handle_type": "uint",
    "hash_type": "uint",
    "vector3_type": "global::Rage.Vector3",
    "player_id_type": "int"
  },
  "accessibility": "public"
}
```
