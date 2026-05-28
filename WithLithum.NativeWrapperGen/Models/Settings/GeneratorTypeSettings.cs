// SPDX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;
using WithLithum.NativeWrapperGen.Generation;
using SCPT = WithLithum.NativeWrapperGen.Models.ScriptCommandParameterType;
using SCRT = WithLithum.NativeWrapperGen.Models.ScriptCommandReturnType;

namespace WithLithum.NativeWrapperGen.Models.Settings;

/// <summary>
/// Configures type mapping for shim generators.
/// </summary>
public sealed record GeneratorTypeSettings
{
    /// <summary>
    /// Gets the type used for <c>Any</c> typed parameters.
    /// </summary>
    public required string AnyParameterType { get; init; }

    /// <summary>
    /// Gets the type used for <c>Any</c> typed pointers.
    /// </summary>
    public required string AnyPointerType { get; init; }

    /// <summary>
    /// Gets the type used for <c>Any</c> typed return values.
    /// </summary>
    public required string AnyReturnType { get; init; }

    /// <summary>
    /// Gets the type used for the handles of the entities.
    /// </summary>
    public required string HandleType { get; init; }

    /// <summary>
    /// Gets the type used for Jenkins-One-At-A-Time (JOAAT) hash values.
    /// </summary>
    public required string HashType { get; init; }

    /// <summary>
    /// Gets the type used for <c>Vector3</c> typed parameters and return values.
    /// </summary>
    [JsonPropertyName("vector3_type")]
    public required string Vector3Type { get; init; }

    /// <summary>
    /// Gets the type used for player identifiers.
    /// </summary>
    public required string PlayerIdType { get; init; }

    /// <summary>
    /// Gets a dictionary that overrides type mapping for return types.
    /// </summary>
    public ReturnTypeConversionTable? ReturnTypeOverrides { get; init; }

    /// <summary>
    /// Gets a dictionary that overrides type mapping for parameter types.
    /// </summary>
    public ParameterTypeConversionTable? ParameterTypeOverrides { get; init; }

    /// <summary>
    /// Maps the specified parameter type to its CLR representation according to the settings of
    /// the current instance.
    /// </summary>
    /// <param name="paramType">The type of the parameter to map.</param>
    /// <returns>
    /// The mapped CLR representation name. If the type cannot be mapped, returns
    /// <see cref="AnyParameterType"/>.
    /// </returns>
    public string GetTypeName(SCPT paramType)
    {
        if (ParameterTypeOverrides?.TryGetValue(paramType, out var overriddenType) == true)
        {
            return overriddenType;
        }

        return paramType switch
        {
            SCPT.Ped
            or SCPT.Vehicle
            or SCPT.Entity
            or SCPT.Blip
            or SCPT.Cam
            or SCPT.FireId
            or SCPT.Pickup
            or SCPT.Object
            or SCPT.ScrHandle => HandleType,

            SCPT.Player => PlayerIdType,
            SCPT.Hash => HashType,
            SCPT.AnyPointer or SCPT.MutableString => AnyPointerType,
            SCPT.Vector3 => Vector3Type,

            // Primitives
            SCPT.String => "string",
            SCPT.Int => "int",
            SCPT.Float => "float",
            SCPT.Void => "void",
            SCPT.Boolean => "bool",

            _ => AnyParameterType
        };
    }

    /// <summary>
    /// Maps the specified return type to its CLR representation according to the settings of the
    /// current instance.
    /// </summary>
    /// <param name="returnType">The return type to map.</param>
    /// <returns>
    /// The mapped CLR representation name. If the type cannot be mapped, returns
    /// <see cref="AnyReturnType"/>.
    /// </returns>
    public string GetTypeName(SCRT returnType)
    {
        if (ReturnTypeOverrides?.TryGetValue(returnType, out var overriddenType) == true)
        {
            return overriddenType;
        }

        return returnType switch
        {
            SCRT.Ped
            or SCRT.Vehicle
            or SCRT.Entity
            or SCRT.Blip
            or SCRT.Cam
            or SCRT.FireId
            or SCRT.Pickup
            or SCRT.Object
            or SCRT.ScrHandle => HandleType,

            SCRT.Player => PlayerIdType,
            SCRT.Hash => HashType,
            SCRT.AnyPointer => AnyPointerType,
            SCRT.Vector3 => Vector3Type,

            // Primitives
            SCRT.String => "string",
            SCRT.Int => "int",
            SCRT.Float => "float",
            SCRT.Void => "void",
            SCRT.Boolean => "bool",

            _ => AnyReturnType
        };
    }

    internal string GetStringForType(ScriptCommandParameterType paramType, bool stripRef = false)
    {
        if (ParamUtil.PointerToRegularMap.TryGetValue(paramType,
                out var resultType))
        {
            var resultTypeName = GetTypeName(resultType);
            return stripRef
                ? resultTypeName
                : $"ref {resultTypeName}";
        }

        return GetTypeName(paramType);
    }
}
