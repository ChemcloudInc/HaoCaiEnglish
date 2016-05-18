using Himall.Core.Strategies;
using System;

namespace Himall.Core
{
	public interface ICache : IStrategy
	{
		int TimeOut
		{
			get;
			set;
		}

		object Get(string key);

		void Insert(string key, object data);

		void Insert(string key, object data, int cacheTime);

		void Insert(string key, object data, DateTime cacheTime);

		void Remove(string key);
	}
}