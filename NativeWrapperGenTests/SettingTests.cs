// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Models;
using WithLithum.NativeWrapperGen.Models.Settings;
using SCPT = WithLithum.NativeWrapperGen.Models.ScriptCommandParameterType;
using SCRT = WithLithum.NativeWrapperGen.Models.ScriptCommandReturnType;

namespace NativeWrapperGenTests;

public class SettingTests
{
    private static readonly GeneratorTypeSettings Base = new()
    {
        AnyParameterType = "__UNDEFINED__",
        AnyPointerType = "__UNDEFINED__",
        AnyReturnType = "__UNDEFINED__",
        HandleType = "__UNDEFINED__",
        PlayerIdType = "__UNDEFINED__",
        Vector3Type = "__UNDEFINED__",
    };

    [Fact]
    public void GetParamTypeName_AnyType_ReturnsHandleType()
    {
        // Arrange
        var settings = Base with { AnyParameterType = "Any" };
        const SCPT input = SCPT.Any;

        // Act
        var result = settings.GetTypeName(input);

        // Assert
        Assert.Equal("Any", result);
    }

    [Fact]
    public void GetReturnTypeName_AnyType_ReturnsHandleType()
    {
        // Arrange
        var settings = Base with { AnyReturnType = "Any" };
        const SCRT input = SCRT.Any;

        // Act
        var result = settings.GetTypeName(input);

        // Assert
        Assert.Equal("Any", result);
    }

    [Fact]
    public void GetParamTypeName_AnyPointerType_ReturnsHandleType()
    {
        // Arrange
        var settings = Base with { HandleType = "PoolHandle" };
        const SCPT input = SCPT.Entity;

        // Act
        var result = settings.GetTypeName(input);

        // Assert
        Assert.Equal("PoolHandle", result);
    }

    [Fact]
    public void GetParamTypeName_HandleableType_ReturnsHandleType()
    {
        // Arrange
        var settings = Base with { HandleType = "PoolHandle" };
        const SCPT input = SCPT.Entity;

        // Act
        var result = settings.GetTypeName(input);

        // Assert
        Assert.Equal("PoolHandle", result);
    }

    [Fact]
    public void GetParamTypeName_OverrideDefined_ReturnsOverride()
    {
        // Arrange
        var settings = Base with
        {
            ParameterTypeOverrides = new Dictionary<SCPT, string>()
            {
                { ScriptCommandParameterType.PickupPointer, "PickupPointer" },
            },
        };
        const SCPT input = SCPT.PickupPointer;

        // Act
        var result = settings.GetTypeName(input);

        // Assert
        Assert.Equal("PickupPointer", result);
    }

    [Fact]
    public void GetReturnTypeName_OverrideDefined_ReturnsOverride()
    {
        // Arrange
        var settings = Base with
        {
            ReturnTypeOverrides = new Dictionary<SCRT, string>()
            {
                { SCRT.Cam, "Camera" },
            },
        };
        const SCRT input = SCRT.Cam;

        // Act
        var result = settings.GetTypeName(input);

        // Assert
        Assert.Equal("Camera", result);
    }
}
