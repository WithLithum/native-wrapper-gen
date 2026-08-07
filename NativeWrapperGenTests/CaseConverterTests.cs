// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Generation;
using WithLithum.NativeWrapperGen.Models.Settings;

namespace NativeWrapperGenTests;

public class CaseConverterTests
{
    [Fact]
    public void SnakeToPascal_RegularUpperPascalString_FormatCorrectly()
    {
        // Arrange
        const string toConvert = "THIS_IS_A_REGULAR_STRING";

        // Act
        var result = MethodNameConverter.SnakeToPascal(toConvert);

        // Assert
        Assert.Equal("ThisIsARegularString", result);
    }

    [Fact]
    public void SnakeToPascalSpan_RegularUpperPascalString_FormatCorrectly()
    {
        // Arrange
        const string toConvert = "THIS_IS_A_REGULAR_STRING";
        Span<char> nameBuf = stackalloc char[toConvert.Length];

        // Act
        var written = MethodNameConverter.SnakeToPascal(toConvert.AsSpan(), nameBuf);

        // Assert
        Assert.Equal("ThisIsARegularString", nameBuf[..written]);
    }

    [Fact]
    public void HashToMethodRage_UpperCaseHash_FormatCorrectly()
    {
        // Arrange
        const string toConvert = "0xA17784FCA9548D15";
        Span<char> nameBuf = stackalloc char[17]; // 18 - 1
        
        // Act
        var written = MethodNameConverter.HashToMethodName(toConvert, 
            nameBuf,
            HashNameStyle.RagePluginHook);
        
        // Assert
        Assert.Equal("xA17784FCA9548D15", nameBuf[..written]);
    }
    
    [Fact]
    public void HashToMethodRage_LowerCaseHash_FormatCorrectly()
    {
        // Arrange
        const string toConvert = "0xa17784fca9548d15";
        Span<char> nameBuf = stackalloc char[17]; // 18 - 1
        
        // Act
        var written = MethodNameConverter.HashToMethodName(toConvert, 
            nameBuf,
            HashNameStyle.RagePluginHook);
        
        // Assert
        Assert.Equal("xA17784FCA9548D15", nameBuf[..written]);
    }
    
    [Fact]
    public void HashToMethodCfx_UpperCaseHash_FormatCorrectly()
    {
        // Arrange
        const string toConvert = "0xA17784FCA9548D15";
        Span<char> nameBuf = stackalloc char[20]; // 18 + 2
        
        // Act
        var written = MethodNameConverter.HashToMethodName(toConvert, 
            nameBuf,
            HashNameStyle.Cfx);
        
        // Assert
        Assert.Equal("N_0xa17784fca9548d15", nameBuf[..written]);
    }
    
    [Fact]
    public void HashToMethodCfx_LowerCaseHash_FormatCorrectly()
    {
        // Arrange
        const string toConvert = "0xa17784fca9548d15";
        Span<char> nameBuf = stackalloc char[20]; // 18 + 2
        
        // Act
        var written = MethodNameConverter.HashToMethodName(toConvert, 
            nameBuf,
            HashNameStyle.Cfx);
        
        // Assert
        Assert.Equal("N_0xa17784fca9548d15", nameBuf[..written]);
    }
}
