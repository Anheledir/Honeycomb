namespace BaseBotService.Data.Extensions
{
    // Extension method to batch a collection into smaller chunks
    public static class BatchExtension
    {
        public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            List<T> batch = new List<T>(size);

            foreach (var item in source)
            {
                batch.Add(item);

                if (batch.Count >= size)
                {
                    yield return new List<T>(batch);
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                yield return new List<T>(batch);
            }
        }
    }
}
