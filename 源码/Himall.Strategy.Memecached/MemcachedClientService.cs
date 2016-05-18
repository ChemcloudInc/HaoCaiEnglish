using Enyim.Caching;
using System;

namespace Himall.Strategy
{
	internal class MemcachedClientService
	{
		private readonly static MemcachedClientService _instance;

		private readonly MemcachedClient _client;

		public MemcachedClient Client
		{
			get
			{
				return _client;
			}
		}

		public static MemcachedClientService Instance
		{
			get
			{
				return MemcachedClientService._instance;
			}
		}

		static MemcachedClientService()
		{
			MemcachedClientService._instance = new MemcachedClientService();
		}

		private MemcachedClientService()
		{
            _client = new MemcachedClient("memcached");
		}
	}
}