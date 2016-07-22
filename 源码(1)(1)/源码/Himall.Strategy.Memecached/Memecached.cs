using Enyim.Caching;
using Enyim.Caching.Memcached;
using Himall.Core;
using System;

namespace Himall.Strategy
{
	public class Memecached : ICache
	{
		private const int DEFAULT_TMEOUT = 600;

		private int timeout = 600;

		private MemcachedClient client = MemcachedClientService.Instance.Client;

		public int TimeOut
		{
			get
			{
				return timeout;
			}
			set
			{
                timeout = (value > 0 ? value : 600);
			}
		}

		public Memecached()
		{
		}

		public void Clear()
		{
            client.FlushAll();
		}

		public object Get(string key)
		{
			return client.Get(key);
		}

		public void Insert(string key, object data)
		{
            client.Store(StoreMode.Add, key, data);
		}

		public void Insert(string key, object data, int cacheTime)
		{
			MemcachedClient memcachedClient = client;
			DateTime now = DateTime.Now;
			memcachedClient.Store(StoreMode.Set, key, data, now.AddMinutes(timeout));
		}

		public void Insert(string key, object data, DateTime cacheTime)
		{
            client.Store(StoreMode.Set, key, data, cacheTime);
		}

		public void Remove(string key)
		{
            client.Remove(key);
		}
	}
}