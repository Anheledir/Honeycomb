namespace BaseBotService.Utilities.Extensions;
/// <summary>
/// Provides extension methods for the List<T> class.
/// </summary>
public static class ListExtensions
{
    private static readonly Random _random = new();

    /// <summary>
    /// Gets a random item from the specified list.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    /// <param name="list">The list from which to get a random item.</param>
    /// <returns>A random item from the list.</returns>
    /// <exception cref="ArgumentException">Thrown if the list is null or empty.</exception>
    public static T GetRandomItem<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            throw new ArgumentException("The list cannot be null or empty.");
        }

        int randomIndex = _random.Next(list.Count);
        return list[randomIndex];
    }
}