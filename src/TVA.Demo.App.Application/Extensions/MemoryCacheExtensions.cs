using Microsoft.Extensions.Caching.Memory;

namespace TVA.Demo.App.Application.Extensions
{
    public static class MemoryCacheExtensions
    {
        public static IEnumerable<string> GetKeysStartingWith(this IMemoryCache cache, string prefix)
        {
            var fieldInfo = cache.GetType().GetField("_entries", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldInfo != null && fieldInfo.GetValue(cache) is System.Collections.IDictionary cacheEntries)
            {
                return cacheEntries.Keys.OfType<string>().Where(key => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            }
            return Enumerable.Empty<string>();
        }
    }
}
