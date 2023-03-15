using BaseBotService.Extensions;
using Discord;

namespace BaseBotService.Tests.Extensions
{
    public class EmbedBuilderExtensionTests
    {
        [Test]
        public void WithFieldIf_ConditionTrue_FieldIsAdded()
        {
            // Arrange
            var embedBuilder = new EmbedBuilder();
            var embedFieldBuilder = new EmbedFieldBuilder().WithName("Name").WithValue("Value");
            bool condition = true;

            // Act
            embedBuilder.WithFieldIf(condition, embedFieldBuilder);

            // Assert
            embedBuilder.Fields.Count.ShouldBe(1);
            embedBuilder.Fields[0].Name.ShouldBe("Name");
            embedBuilder.Fields[0].Value.ShouldBe("Value");
        }

        [Test]
        public void WithFieldIf_ConditionFalse_FieldIsNotAdded()
        {
            // Arrange
            var embedBuilder = new EmbedBuilder();
            var embedFieldBuilder = new EmbedFieldBuilder().WithName("Name").WithValue("Value");
            bool condition = false;

            // Act
            embedBuilder.WithFieldIf(condition, embedFieldBuilder);

            // Assert
            embedBuilder.Fields.Count.ShouldBe(0);
        }
    }
}
