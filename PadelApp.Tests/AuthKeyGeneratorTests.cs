using PadelApp.Services;

namespace PadelApp.Tests;

public class AuthKeyGeneratorTests
{
    [Theory]
    [InlineData("ExpectedGeneratedKey")]
    [InlineData("ShortKey")]
    [InlineData("ThisIsAMuchLongerKeyToTest")]
    [InlineData("Special@#$%Characters")]
    public void EncryptAndDecryptKey_OriginalAndDecryptedShouldBeTheSame_ShouldSucceed(string expectedKey)
    {
        // Act
        var generatedKey = AuthKeyGenerator.GenerateEncryptedKey(expectedKey);
        var decryptedKey = AuthKeyGenerator.Decrypt(generatedKey);

        // Assert
        Assert.Equal(expectedKey, decryptedKey);
    }

    [Fact]
    public void EncryptKey_ShouldGenerateDifferentKeysForSameInput()
    {
        // Arrange
        var input = "SameKey";

        // Act
        var firstKey = AuthKeyGenerator.GenerateEncryptedKey(input);
        var secondKey = AuthKeyGenerator.GenerateEncryptedKey(input);

        // Assert
        Assert.NotEqual(firstKey, secondKey); // Due to different IVs
        Assert.Equal(input, AuthKeyGenerator.Decrypt(firstKey));
        Assert.Equal(input, AuthKeyGenerator.Decrypt(secondKey));
    }
}