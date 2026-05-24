// SDPX-FileCopyrightText: 2025-2026 WithLithum
// SPDX-License-Identifier: Apache-2.0

using WithLithum.NativeWrapperGen.Generation;

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
}