using BaseBotService.Extensions;

namespace BaseBotService.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("a", "*")]
        [TestCase("ab", "*")]
        [TestCase("abc", "a*c")]
        [TestCase("abcd", "a*d")]
        [TestCase("abcdefgh", "ab**gh")]
        public void MaskToken_ReturnsMaskedToken(string input, string expectedMaskedToken)
        {
            // Act
            var maskedToken = input.MaskToken();

            // Assert
            maskedToken.ShouldBe(expectedMaskedToken);
        }
    }
}
