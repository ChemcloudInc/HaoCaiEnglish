using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class HomeCategoryService : ServiceBase, IHomeCategoryService, IService, IDisposable
	{
		private const int HOME_CATEGORY_SET_COUNT = 14;

		public int TotalRowsCount
		{
			get
			{
				return 14;
			}
		}

		public HomeCategoryService()
		{
		}

		public HomeCategorySet GetHomeCategorySet(int rowNumber)
		{
			if (rowNumber > 14 || rowNumber < 0)
			{
				throw new ArgumentNullException(string.Concat("行号不在取值范围内！取值必须大于0且小于", 14));
			}
			HomeCategorySet homeCategorySet = new HomeCategorySet()
			{
				RowNumber = rowNumber,
				HomeCategories = context.HomeCategoryInfo.FindBy((HomeCategoryInfo item) => item.RowNumber == rowNumber)
			};
			HomeCategoryRowInfo homeCategoryRowInfo = context.HomeCategoryRowInfo.FindBy((HomeCategoryRowInfo item) => item.RowId == rowNumber).FirstOrDefault();
			List<HomeCategorySet.HomeCategoryTopic> homeCategoryTopics = new List<HomeCategorySet.HomeCategoryTopic>();
			HomeCategorySet.HomeCategoryTopic homeCategoryTopic = new HomeCategorySet.HomeCategoryTopic()
			{
				Url = homeCategoryRowInfo.Url1,
				ImageUrl = homeCategoryRowInfo.Image1
			};
			homeCategoryTopics.Add(homeCategoryTopic);
			HomeCategorySet.HomeCategoryTopic homeCategoryTopic1 = new HomeCategorySet.HomeCategoryTopic()
			{
				Url = homeCategoryRowInfo.Url2,
				ImageUrl = homeCategoryRowInfo.Image2
			};
			homeCategoryTopics.Add(homeCategoryTopic1);
			homeCategorySet.HomeCategoryTopics = homeCategoryTopics;
			return homeCategorySet;
		}

		public IEnumerable<HomeCategorySet> GetHomeCategorySets()
		{
			HomeCategorySet[] homeCategorySetArray = new HomeCategorySet[14];
			if (Cache.Get("Cache-HomeCategories") == null)
			{
				List<HomeCategoryInfo> list = (
					from C in context.HomeCategoryInfo.Include("CategoryInfo")
					select C).ToList();
				List<IGrouping<int, HomeCategoryInfo>> groupings = (
					from C in list
					group C by C.RowNumber into G
					select G).ToList<IGrouping<int, HomeCategoryInfo>>();
				foreach (IGrouping<int, HomeCategoryInfo> nums in groupings)
				{
					HomeCategorySet homeCategorySet = new HomeCategorySet()
					{
						RowNumber = nums.Key,
						HomeCategories = nums
					};
					homeCategorySetArray[nums.Key - 1] = homeCategorySet;
				}
				List<HomeCategoryRowInfo> homeCategoryRowInfos = context.HomeCategoryRowInfo.FindAll<HomeCategoryRowInfo>().ToList();
				for (int i = 0; i < 14; i++)
				{
					if (homeCategorySetArray[i] == null)
					{
						int num = i;
						HomeCategorySet homeCategorySet1 = new HomeCategorySet()
						{
							RowNumber = i + 1,
							HomeCategories = new List<HomeCategoryInfo>()
						};
						homeCategorySetArray[num] = homeCategorySet1;
					}
					List<HomeCategorySet.HomeCategoryTopic> homeCategoryTopics = new List<HomeCategorySet.HomeCategoryTopic>();
					HomeCategoryRowInfo homeCategoryRowInfo = (
						from item in homeCategoryRowInfos
						where item.RowId == i + 1
						select item).FirstOrDefault();
					if (homeCategoryRowInfo != null)
					{
						HomeCategorySet.HomeCategoryTopic homeCategoryTopic = new HomeCategorySet.HomeCategoryTopic()
						{
							Url = homeCategoryRowInfo.Url1,
							ImageUrl = homeCategoryRowInfo.Image1
						};
						homeCategoryTopics.Add(homeCategoryTopic);
						HomeCategorySet.HomeCategoryTopic homeCategoryTopic1 = new HomeCategorySet.HomeCategoryTopic()
						{
							Url = homeCategoryRowInfo.Url2,
							ImageUrl = homeCategoryRowInfo.Image2
						};
						homeCategoryTopics.Add(homeCategoryTopic1);
					}
					homeCategorySetArray[i].HomeCategoryTopics = homeCategoryTopics;
				}
				Cache.Insert("Cache-HomeCategories", homeCategorySetArray, 5);
			}
			else
			{
				homeCategorySetArray = (HomeCategorySet[])Cache.Get("Cache-HomeCategories");
			}
			return homeCategorySetArray;
		}

		private string TransferImages(string oriImageUrl)
		{
			string str = "/Storage/Plat/PageSettings/HomeCategory";
			string mapPath = IOHelper.GetMapPath(str);
			if (!Directory.Exists(mapPath))
			{
				Directory.CreateDirectory(mapPath);
			}
			string str1 = oriImageUrl;
			if (!oriImageUrl.Contains("newDir"))
			{
				string str2 = oriImageUrl.Substring(oriImageUrl.LastIndexOf('.'));
				Guid guid = Guid.NewGuid();
				string str3 = string.Concat(guid.ToString("N"), str2);
				string str4 = string.Concat(IOHelper.GetMapPath(str), "/", str3);
				string mapPath1 = IOHelper.GetMapPath(oriImageUrl);
				str1 = string.Concat(str, str3);
				File.Copy(mapPath1, str4, true);
			}
			return str1;
		}

		public void UpdateHomeCategorySet(HomeCategorySet homeCategorySet)
		{
			if (homeCategorySet.HomeCategories == null)
			{
				throw new ArgumentNullException("传入的分类不能为null，但可以是空集合");
			}
			int rowNumber = homeCategorySet.HomeCategories.FirstOrDefault().RowNumber;
			if (rowNumber > 14 || rowNumber < 0)
			{
				throw new ArgumentNullException(string.Concat("行号不在取值范围内！取值必须大于0且小于", 14));
			}
            context.HomeCategoryInfo.OrderBy((HomeCategoryInfo item) => item.RowNumber == rowNumber);
			foreach (HomeCategoryInfo list in homeCategorySet.HomeCategories.ToList())
			{
				list.RowNumber = rowNumber;
			}
            context.HomeCategoryInfo.AddRange(homeCategorySet.HomeCategories);
            context.SaveChanges();
		}

		public void UpdateHomeCategorySet(int rowNumber, IEnumerable<long> categoryIds)
		{
			if (rowNumber > 14 || rowNumber < 0)
			{
				throw new ArgumentNullException(string.Concat("行号不在取值范围内！取值必须大于0且小于", 14));
			}
			CategoryService categoryService = new CategoryService();
			int num = categoryIds.Count();
			HomeCategoryInfo[] homeCategoryInfoArray = new HomeCategoryInfo[num];
			for (int i = 0; i < num; i++)
			{
				long num1 = categoryIds.ElementAt<long>(i);
				HomeCategoryInfo homeCategoryInfo = new HomeCategoryInfo()
				{
					RowNumber = rowNumber,
					CategoryId = num1,
					Depth = categoryService.GetCategory(num1).Depth
				};
				homeCategoryInfoArray[i] = homeCategoryInfo;
			}
            context.HomeCategoryInfo.OrderBy((HomeCategoryInfo item) => item.RowNumber == rowNumber);
            context.HomeCategoryInfo.AddRange(homeCategoryInfoArray);
            context.SaveChanges();
		}

		public void UpdateHomeCategorySet(int rowNumber, IEnumerable<HomeCategorySet.HomeCategoryTopic> homeCategoryTopics)
		{
			HomeCategoryRowInfo imageUrl;
			if (rowNumber > 14 || rowNumber < 0)
			{
				throw new ArgumentNullException(string.Concat("行号不在取值范围内！取值必须大于0且小于", 14));
			}
			HomeCategoryRowInfo homeCategoryRowInfo = context.HomeCategoryRowInfo.FindBy((HomeCategoryRowInfo item) => item.RowId == rowNumber).FirstOrDefault();
			imageUrl = (homeCategoryRowInfo != null ? homeCategoryRowInfo : new HomeCategoryRowInfo()
			{
				RowId = rowNumber
			});
			int num = 0;
			string[] image2 = new string[2];
			foreach (HomeCategorySet.HomeCategoryTopic list in homeCategoryTopics.ToList<HomeCategorySet.HomeCategoryTopic>())
			{
				if (string.IsNullOrWhiteSpace(list.Url) || string.IsNullOrWhiteSpace(list.ImageUrl))
				{
					continue;
				}
				int num1 = num;
				num = num1 + 1;
				if (num1 != 0)
				{
					if (imageUrl.Image2 != list.ImageUrl)
					{
						image2[1] = imageUrl.Image2;
					}
					imageUrl.Image2 = list.ImageUrl;
					imageUrl.Url2 = list.Url;
				}
				else
				{
					if (imageUrl.Image1 != list.ImageUrl)
					{
						image2[0] = imageUrl.Image1;
					}
					imageUrl.Image1 = list.ImageUrl;
					imageUrl.Url1 = list.Url;
				}
				if (string.IsNullOrWhiteSpace(list.ImageUrl))
				{
					continue;
				}
                TransferImages(list.ImageUrl);
			}
			if (homeCategoryRowInfo == null)
			{
                context.HomeCategoryRowInfo.Add(imageUrl);
			}
            context.SaveChanges();
			string[] strArrays = image2;
			for (int i = 0; i < strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (!string.IsNullOrWhiteSpace(str))
				{
					File.Delete(IOHelper.GetMapPath(str));
				}
			}
		}

		public void UpdateHomeCategorySetSequence(int sourceRowNumber, int destiRowNumber)
		{
			if (sourceRowNumber > 14 || sourceRowNumber < 0)
			{
				throw new ArgumentNullException(string.Concat("原行号不在取值范围内！取值必须大于0且小于", 14));
			}
			if (destiRowNumber > 14 || destiRowNumber < 0)
			{
				throw new ArgumentNullException(string.Concat("新行号不在取值范围内！取值必须大于0且小于", 14));
			}
			IQueryable<HomeCategoryInfo> homeCategoryInfos = context.HomeCategoryInfo.FindBy((HomeCategoryInfo item) => item.RowNumber == sourceRowNumber);
			IQueryable<HomeCategoryInfo> homeCategoryInfos1 = context.HomeCategoryInfo.FindBy((HomeCategoryInfo item) => item.RowNumber == destiRowNumber);
			foreach (HomeCategoryInfo list in homeCategoryInfos.ToList())
			{
				list.RowNumber = destiRowNumber;
			}
			foreach (HomeCategoryInfo homeCategoryInfo in homeCategoryInfos1.ToList())
			{
				homeCategoryInfo.RowNumber = sourceRowNumber;
			}
            context.SaveChanges();
		}
	}
}