using Himall.Core;
using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Himall.Strategy
{
	public class AspNetCache : ICache
	{
		private const int DEFAULT_TMEOUT = 600;

		private System.Web.Caching.Cache cache;

		private static object cacheLocker;

		private int _timeout = 600;

		public int TimeOut
		{
			get
			{
				return _timeout;
			}
			set
			{
                _timeout = (value > 0 ? value : 600);
			}
		}

		static AspNetCache()
		{
			AspNetCache.cacheLocker = new object();
		}

		public AspNetCache()
		{
            cache = HttpRuntime.Cache;
		}

		public void Clear()
		{
			IDictionaryEnumerator enumerator = cache.GetEnumerator();
			while (enumerator.MoveNext())
			{
                cache.Remove(enumerator.Key.ToString());
			}
		}

		public object Get(string key)
		{
			return cache.Get(key);
		}

		public void Insert(string key, object value)
		{
			lock (AspNetCache.cacheLocker)
			{
				if (cache.Get(key) != null)
				{
                    cache.Remove(key);
				}
                cache.Insert(key, value);
			}
		}

		public void Insert(string key, object value, int cacheTime)
		{
            System.Web.Caching.Cache caches = cache;
			DateTime now = DateTime.Now;
			caches.Insert(key, value, null, now.AddSeconds(cacheTime), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
		}

		public void Insert(string key, object value, DateTime cacheTime)
		{
            cache.Insert(key, value, null, cacheTime, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
		}

		public void Remove(string key)
		{
            cache.Remove(key);
		}
	}
}