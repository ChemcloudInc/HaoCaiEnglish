using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;

namespace Himall.IServices
{
	public interface ISensitiveWordService : IService, IDisposable
	{
		void AddSensitiveWord(SensitiveWordsInfo model);

		void BatchDeleteSensitiveWord(int[] ids);

		void DeleteSensitiveWord(int id);

		bool ExistSensitiveWord(string word);

		IEnumerable<string> GetCategories();

		SensitiveWordsInfo GetSensitiveWord(int id);

		PageModel<SensitiveWordsInfo> GetSensitiveWords(SensitiveWordQuery query);

		void UpdateSensitiveWord(SensitiveWordsInfo model);
	}
}