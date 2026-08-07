// SPDX-FileCopyrightText:2025-2026 WithLithum
// SPDX-License-Identifier:Apache-2.0

using WithLithum.NativeWrapperGen.Generation;
using WithLithum.NativeWrapperGen.Generation.Hooks;
using WithLithum.NativeWrapperGen.Models;
using WithLithum.NativeWrapperGen.Models.Settings;

namespace NativeWrapperGenTests;

public class GeneratorTest
{
    private static ScriptCommandInfo CreateNoParams(string? name,
        string? symbolName = null)
    {
        return new ScriptCommandInfo
        {
            Build = "b223",
            JenkinsHash = "1234567890",
            Name = name,
            SymbolName = symbolName,
            Parameters = [],
            ReturnType = ScriptCommandReturnType.Void
        };
    }

    private static WrapperEmitContext CreateContext(ScriptCommandInfo info,
        string hash)
    {
        return new WrapperEmitContext
        {
            Namespace = "TEST",
            CommandInfo = info,
            Hash = hash,
            ReturnTypeString = "void"
        };
    }
    
    [Fact]
    public void WriteMethodSignature_WithOverride_Correct()
    {
        // Arrange
        var info = CreateNoParams("TEST_NATIVE",
            "TestNativeOverriden");
        var generator = new VDotNetGenerator(VDotNetGenerator.DefaultSettings);
        var context = CreateContext(info, "0xAC2890471901861C");

        var writer = new StringWriter();

        // Act
        generator.WriteMethodSignature(context, writer);
        
        // Assert
        Assert.Equal("public static void TestNativeOverriden()", writer.ToString());
    }
    
    [Fact]
    public void WriteMethodSignature_WithName_Correct()
    {
        // Arrange
        var info = CreateNoParams("TEST_NATIVE");
        var generator = new VDotNetGenerator(VDotNetGenerator.DefaultSettings);
        var context = CreateContext(info, "0xAC2890471901861C");

        var writer = new StringWriter();

        // Act
        generator.WriteMethodSignature(context, writer);
        
        // Assert
        Assert.Equal("public static void TestNative()", writer.ToString());
    }
    
    [Fact]
    public void WriteMethodSignature_WithHashOnly_Correct()
    {
        // Arrange
        var info = CreateNoParams(null);
        var generator = new VDotNetGenerator(VDotNetGenerator.DefaultSettings);
        var context = CreateContext(info, "0xAC2890471901861C");

        var writer = new StringWriter();

        // Act
        generator.WriteMethodSignature(context, writer);
        
        // Assert
        Assert.Equal("public static void xAC2890471901861C()", writer.ToString());
    }
    
    [Fact]
    public void WriteMethodSignature_WithHashOnlyCfxStyle_Correct()
    {
        // Arrange
        ScriptCommandInfo info = CreateNoParams(null);
        var generator = new VDotNetGenerator(VDotNetGenerator.DefaultSettings with
        {
            HashNameStyle = HashNameStyle.Cfx
        });
        var context = CreateContext(info, "0xAC2890471901861C");

        var writer = new StringWriter();

        // Act
        generator.WriteMethodSignature(context, writer);
        
        // Assert
        Assert.Equal("public static void N_0xac2890471901861c()", writer.ToString());
    }
}
