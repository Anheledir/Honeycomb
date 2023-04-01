namespace BaseBotService.Tests;

/// <summary>
/// A helper class for generating fake data, such as random emoji and lists of random ulong values.
/// </summary>
public static class FakeDataHelper
{
    /// <summary>
    /// Generates a list of random ulong values with a random length between the specified min and max bounds (inclusive).
    /// </summary>
    /// <param name="min">The minimum number of ulong values to include in the list. Default is 0.</param>
    /// <param name="max">The maximum number of ulong values to include in the list. Default is 4.</param>
    /// <returns>A list of random ulong values with a length between the specified min and max bounds.</returns>
    /// <example>
    /// <code>
    /// List<ulong> randomUlongList = GenerateRandomUlongList(2, 6);
    /// </code>
    /// </example>
    public static List<ulong> GenerateRandomUlongList(int min = 0, int max = 4)
    {
        var faker = new Faker();
        int listSize = faker.Random.Int(min, max); // Randomly choose the size of the list

        List<ulong> ulongList = new();

        for (int i = 0; i < listSize; i++)
        {
            ulong randomUlong = faker.Random.ULong();
            ulongList.Add(randomUlong);
        }

        return ulongList;
    }

    /// <summary>
    /// Generates a random emoji character from a predefined set of emoji code points.
    /// </summary>
    /// <returns>A random emoji character as a string.</returns>
    /// <example>
    /// <code>
    /// string randomEmoji = RandomEmoji();
    /// </code>
    /// </example>
    public static string RandomEmoji()
    {
        var random = new Randomizer();

        int[] emojiCodePoints = new int[]
        {
            0x1F4B0, // 💰 Money Bag
            0x1F4B2, // 💲 Heavy Dollar Sign
            0x1F4B4, // 💴 Yen Banknote
            0x1F4B5, // 💵 Dollar Banknote
            0x1F4B6, // 💶 Euro Banknote
            0x1F4B7, // 💷 Pound Banknote
            0x1F4B8, // 💸 Flying Banknote
            0x1F4B3, // 💳 Credit Card
            0x1FA99, // 🪙 Coin
            0x1F911, // 🤑 Money-Mouth Face
            0x1F47B, // 👻 Ghost (as "Phantom Coins")
            0x1F47E, // 👾 Alien Monster (as "Alien Coins")
            0x1F48E, // 💎 Gem Stone
            0x1F916, // 🤖 Robot (as "Robot Coins")
            0x1F31F, // 🌟 Glowing Star (as "Star Coins")
            0x1F381, // 🎁 Wrapped Gift (as "Gift Coins")
            0x1F52E, // 🔮 Crystal Ball (as "Magic Coins")
            0x1F36F, // 🍯 Honey Pot
            0x1F36A, // 🍪 Cookie
            0x1F34C, // 🍌 Banana
            0x1F95C, // 🥜 Peanuts
            0x1F36B, // 🍫 Chocolate Bar
            0x1F347, // 🍇 Grapes
            0x1F352, // 🍒 Cherries
            0x1F34D, // 🍍 Pineapple
            0x1F950, // 🥐 Croissant
            0x1F363, // 🍣 Sushi
            0x1F964, // 🥤 Soft Drink
            0x1F377, // 🍷 Wine Glass
            0x1F378, // 🍸 Cocktail Glass
            0x1F37A, // 🍺 Beer Mug
            0x1F37E, // 🍾 Bottle with Popping Cork
            0x1F36D, // 🍭 Lollipop
            0x1F4A1, // 💡 Light Bulb
            0x1F6E2, // 🛢️ Oil Drum
            0x1F48A, // 💊 Pill
            0x1F3E6, // 🏦 Bank
            0x1F4AA, // 💪 Flexed Biceps (as "Power Points")
        };

        // Pick a random emoji code point from the list
        int randomEmojiCodePoint = random.ArrayElement(emojiCodePoints);

        // Convert the code point to a Unicode character
        return char.ConvertFromUtf32(randomEmojiCodePoint);
    }
}