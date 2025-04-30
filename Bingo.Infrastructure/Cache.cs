using Microsoft.Extensions.Caching.Memory;

namespace Bingo.Infrastructure;

internal static class Cache
{
    internal static readonly MemoryCache Instance = new(new MemoryCacheOptions(){ExpirationScanFrequency = new TimeSpan(0, 15, 0)});
}