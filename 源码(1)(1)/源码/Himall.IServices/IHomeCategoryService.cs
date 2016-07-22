using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface IHomeCategoryService : IService, IDisposable
	{
		int TotalRowsCount
		{
			get;
		}

		HomeCategorySet GetHomeCategorySet(int rowNumber);

		IEnumerable<HomeCategorySet> GetHomeCategorySets();

		void UpdateHomeCategorySet(HomeCategorySet homeCategoryset);

		void UpdateHomeCategorySet(int rowNumber, IEnumerable<long> categoryIds);

		void UpdateHomeCategorySet(int rowNumber, IEnumerable<HomeCategorySet.HomeCategoryTopic> homeCategoryTopic);

		void UpdateHomeCategorySetSequence(int sourceRowNumber, int destiRowNumber);
	}
}