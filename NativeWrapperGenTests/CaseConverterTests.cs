namespace NativeWrapperGenTests;

using WithLithum.NativeWrapperGen.Generation;

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