using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace Himall.Service
{
	public class TopicService : ServiceBase, ITopicService, IService, IDisposable
	{
		public TopicService()
		{
		}

		public void AddTopic(TopicInfo topicInfo)
		{
			if (string.IsNullOrWhiteSpace(topicInfo.Name))
			{
				throw new ArgumentNullException("专题名称不能为空");
			}
			string empty = string.Empty;
			TransactionScope transactionScope = new TransactionScope();
			try
			{
				try
				{
                    context.TopicInfo.Add(topicInfo);
                    context.SaveChanges();
					string topImage = topicInfo.TopImage;
					string backgroundImage = topicInfo.BackgroundImage;
					string frontCoverImage = topicInfo.FrontCoverImage;
					empty = MoveImages(topicInfo.Id, ref backgroundImage, ref topImage, ref frontCoverImage);
					topicInfo.TopImage = topImage;
					topicInfo.BackgroundImage = backgroundImage;
					topicInfo.FrontCoverImage = frontCoverImage;
                    context.SaveChanges();
					transactionScope.Complete();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (!string.IsNullOrWhiteSpace(empty))
					{
						try
						{
							Directory.Delete(empty);
						}
						catch
						{
						}
					}
					throw new HimallException("创建专题失败", exception);
				}
			}
			finally
			{
				transactionScope.Dispose();
			}
		}

		public void DeleteTopic(long id)
		{
            context.TopicInfo.Remove(new object[] { id });
			string str = string.Concat(IOHelper.GetMapPath("/Storage/Plat/Topic"), "/", id);
			if (Directory.Exists(str))
			{
				Directory.Delete(str, true);
			}
			if ((
				from item in context.MobileHomeTopicsInfo
				where item.TopicId == id
				select item).Count() > 0)
			{
				throw new HimallException("你的微信首页推荐专题选择了该专题，请先解除选定再删除！");
			}
            context.SaveChanges();
		}

		public TopicInfo GetTopicInfo(long id)
		{
			TopicInfo list = context.TopicInfo.FindById<TopicInfo>(id);
			if (list != null)
			{
				list.TopicModuleInfo = (
					from item in context.TopicModuleInfo
					where item.TopicId == list.Id
					select item).ToList();
			}
			return list;
		}

		public PageModel<TopicInfo> GetTopics(int pageNo, int pageSize, PlatformType platformType = 0)
		{
			int num;
			PageModel<TopicInfo> pageModel = new PageModel<TopicInfo>()
			{
				Models = context.TopicInfo.FindBy<TopicInfo, long>((TopicInfo item) => (int)item.PlatForm == (int)platformType, pageNo, pageSize, out num, (TopicInfo item) => item.Id, false),
				Total = num
			};
			return pageModel;
		}

		public PageModel<TopicInfo> GetTopics(TopicQuery topicQuery)
		{
			int num;
			PageModel<TopicInfo> pageModel = new PageModel<TopicInfo>();
			IQueryable<TopicInfo> topicInfo = 
				from item in context.TopicInfo
				where (int)item.PlatForm == (int)topicQuery.PlatformType
				select item;
			if (topicQuery.ShopId > 0)
			{
				topicInfo = 
					from item in topicInfo
					where item.ShopId == topicQuery.ShopId
					select item;
			}
			if (topicQuery.IsRecommend.HasValue)
			{
				topicInfo = 
					from item in topicInfo
					where item.IsRecommend == topicQuery.IsRecommend.Value
					select item;
			}
			if (!string.IsNullOrWhiteSpace(topicQuery.Name))
			{
				topicQuery.Name = topicQuery.Name.Trim();
				topicInfo = 
					from item in topicInfo
					where item.Name.Contains(topicQuery.Name)
					select item;
			}
			if (!string.IsNullOrWhiteSpace(topicQuery.Tags))
			{
				topicQuery.Tags = topicQuery.Tags.Trim();
				topicInfo = 
					from item in topicInfo
					where item.Tags.Contains(topicQuery.Tags)
					select item;
			}
			Func<IQueryable<TopicInfo>, IOrderedQueryable<TopicInfo>> orderBy = topicInfo.GetOrderBy((IQueryable<TopicInfo> d) => 
				from o in d
				orderby o.Id descending
				select o);
			if (topicQuery.IsAsc)
			{
				orderBy = topicInfo.GetOrderBy((IQueryable<TopicInfo> d) => 
					from o in d
					orderby o.Id
					select o);
			}
			pageModel.Models = topicInfo.GetPage(out num, topicQuery.PageNo, topicQuery.PageSize, orderBy);
			pageModel.Total = num;
			return pageModel;
		}

		private string MoveImages(long topicId, ref string backGroundImage, ref string topImage, ref string frontImage)
		{
			string empty = string.Empty;
			empty = string.Concat(IOHelper.GetMapPath("/Storage/Plat/Topic"), "/", topicId);
			string str = string.Concat("/Storage/Plat/Topic/", topicId, "/");
			if (!Directory.Exists(empty))
			{
				Directory.CreateDirectory(empty);
			}
			if (backGroundImage.Replace("\\", "/").Contains("/temp/"))
			{
				string mapPath = IOHelper.GetMapPath(backGroundImage);
				IOHelper.CopyFile(mapPath, empty, false, "");
				backGroundImage = string.Concat(str, (new FileInfo(mapPath)).Name);
			}
			if (topImage.Replace("\\", "/").Contains("/temp/"))
			{
				string mapPath1 = IOHelper.GetMapPath(topImage);
				IOHelper.CopyFile(mapPath1, empty, false, "");
				topImage = string.Concat(str, (new FileInfo(mapPath1)).Name);
			}
			if (!string.IsNullOrWhiteSpace(frontImage) && frontImage.Replace("\\", "/").Contains("/temp/"))
			{
				string str1 = IOHelper.GetMapPath(frontImage);
				IOHelper.CopyFile(str1, empty, false, "");
				frontImage = string.Concat(str, (new FileInfo(str1)).Name);
			}
			return empty;
		}

		public void UpdateTopic(TopicInfo topicInfo)
		{
			string empty = string.Empty;
			try
			{
				string topImage = topicInfo.TopImage;
				string backgroundImage = topicInfo.BackgroundImage;
				string frontCoverImage = topicInfo.FrontCoverImage;
				empty = MoveImages(topicInfo.Id, ref backgroundImage, ref topImage, ref frontCoverImage);
				List<TopicModuleInfo> topicModuleInfos = new List<TopicModuleInfo>();
				TopicModuleInfo[] array = topicInfo.TopicModuleInfo.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					TopicModuleInfo topicModuleInfo = array[i];
					TopicModuleInfo topicModuleInfo1 = new TopicModuleInfo()
					{
						Name = topicModuleInfo.Name,
						TopicId = topicModuleInfo.TopicId,
						ModuleProductInfo = topicModuleInfo.ModuleProductInfo.ToList()
					};
					topicModuleInfos.Add(topicModuleInfo1);
				}
                //context.TopicModuleInfo.OrderBy((TopicModuleInfo item) => item.TopicId == topicInfo.Id);
                context.TopicModuleInfo.Remove<TopicModuleInfo>((TopicModuleInfo item) => item.TopicId == topicInfo.Id);
                context.SaveChanges();

				TopicInfo name = context.TopicInfo.FindById<TopicInfo>(topicInfo.Id);
				name.Name = topicInfo.Name;
				name.BackgroundImage = backgroundImage;
				name.TopImage = topImage;
				name.FrontCoverImage = frontCoverImage;
				name.Tags = topicInfo.Tags;
				name.TopicModuleInfo = topicModuleInfos;
				name.IsRecommend = topicInfo.IsRecommend;
				name.SelfDefineText = topicInfo.SelfDefineText;
                context.SaveChanges();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (!string.IsNullOrWhiteSpace(empty))
				{
					try
					{
						Directory.Delete(empty);
					}
					catch
					{
					}
				}
				throw new HimallException("修改专题失败", exception);
			}
		}
	}
}