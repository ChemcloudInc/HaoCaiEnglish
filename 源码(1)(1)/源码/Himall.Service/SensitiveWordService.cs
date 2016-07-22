using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class SensitiveWordService : ServiceBase, ISensitiveWordService, IService, IDisposable
	{
		public SensitiveWordService()
		{
		}

		public void AddSensitiveWord(SensitiveWordsInfo model)
		{
            context.SensitiveWordsInfo.Add(model);
            context.SaveChanges();
		}

		public void BatchDeleteSensitiveWord(int[] ids)
		{
			IQueryable<SensitiveWordsInfo> sensitiveWordsInfos = context.SensitiveWordsInfo.FindBy((SensitiveWordsInfo item) => ids.Contains<int>(item.Id));
            context.SensitiveWordsInfo.RemoveRange(sensitiveWordsInfos);
            context.SaveChanges();
		}

		public void DeleteSensitiveWord(int id)
		{
			SensitiveWordsInfo sensitiveWordsInfo = context.SensitiveWordsInfo.FindById<SensitiveWordsInfo>(id);
            context.SensitiveWordsInfo.Remove(sensitiveWordsInfo);
            context.SaveChanges();
		}

		public bool ExistSensitiveWord(string word)
		{
			if ((
				from item in context.SensitiveWordsInfo
				where item.SensitiveWord.Trim().ToLower() == word.Trim().ToLower()
				select item).Count() > 0)
			{
				return true;
			}
			return false;
		}

		public IEnumerable<string> GetCategories()
		{
			IQueryable<string> strs = (
				from item in context.SensitiveWordsInfo
				select item.CategoryName).Distinct<string>();
			return strs;
		}

		public SensitiveWordsInfo GetSensitiveWord(int id)
		{
			return context.SensitiveWordsInfo.FindById<SensitiveWordsInfo>(id);
		}

		public PageModel<SensitiveWordsInfo> GetSensitiveWords(SensitiveWordQuery query)
		{
			int num = 0;
			IQueryable<SensitiveWordsInfo> page = context.SensitiveWordsInfo.AsQueryable<SensitiveWordsInfo>();
			if (!string.IsNullOrWhiteSpace(query.SensitiveWord))
			{
				page = 
					from item in page
					where item.SensitiveWord.Contains(query.SensitiveWord)
					select item;
			}
			if (!string.IsNullOrWhiteSpace(query.CategoryName))
			{
				page = 
					from item in page
					where item.CategoryName.Contains(query.CategoryName)
					select item;
			}
			page = page.GetPage(out num, query.PageNo, query.PageSize, null);
			return new PageModel<SensitiveWordsInfo>()
			{
				Models = page,
				Total = num
			};
		}

		public void UpdateSensitiveWord(SensitiveWordsInfo model)
		{
			SensitiveWordsInfo sensitiveWord = GetSensitiveWord(model.Id);
			sensitiveWord.SensitiveWord = model.SensitiveWord;
			sensitiveWord.CategoryName = model.CategoryName;
            context.SaveChanges();
		}
	}
}