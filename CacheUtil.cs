using System.Runtime.Caching;

namespace MVC5AutoVersioningSample.Utils
{
    public static class CacheUtil
    {
        #region SampleCode for Cash Signaled Item
        public static void SetSignaledCash(string key, string value)
        {
            var cache = MemoryCache.Default;
            var versionItem = cache.GetCacheItem(key);

            if (versionItem == null)
            {

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = System.DateTimeOffset.UtcNow.AddHours(1);
                policy.ChangeMonitors.Add(new SignaledChangeMonitor(key));
                versionItem = new CacheItem(key, value);
                //캐시 설정
                cache.Set(versionItem, policy);
            }
        }

        public static void FlushSignaledCash(string key)
        {
            SignaledChangeMonitor.Signal(key);
        }
        #endregion
    }
 }
