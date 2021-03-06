using System.Runtime.Caching;

namespace LCTM.Utils
{
    public static class CacheUtil
    {
        #region SampleCode for Cash Signaled Item
        public static void SetSignaledCash(string key, object value)
        {
            var cache = MemoryCache.Default;
            var versionItem = cache.GetCacheItem(key);

            if (versionItem == null)
            {

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = System.DateTimeOffset.UtcNow.AddHours(1);
                policy.ChangeMonitors.Add(new SignaledChangeMonitor(key, key));
                versionItem = new CacheItem(key, value);
                //캐시 설정
                cache.Set(versionItem, policy);
            }
        }

        public static object GetSignaledCash(string key)
        {
            var cache = MemoryCache.Default;
            var versionItem = cache.GetCacheItem(key);

            return versionItem?.Value;
        }

        public static void FlushSignaledCash(string key = null)
        {
            SignaledChangeMonitor.Signal(key);
        }
        #endregion
    }
}
