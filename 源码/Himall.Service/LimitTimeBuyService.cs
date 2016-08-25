using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;


namespace Himall.Service
{
	public class LimitTimeBuyService : ServiceBase, ILimitTimeBuyService, IService, IDisposable
	{
		public LimitTimeBuyService()
		{
		}

		public void AddLimitTimeItem(LimitTimeMarketInfo model)
		{
            CheckLimit(model);
            context.LimitTimeMarketInfo.Add(model);
            context.SaveChanges();
		}

		private void AddMarketServiceRecord(long msId, DateTime start, DateTime end)
		{
			DbSet<MarketServiceRecordInfo> marketServiceRecordInfo = context.MarketServiceRecordInfo;
			MarketServiceRecordInfo marketServiceRecordInfo1 = new MarketServiceRecordInfo()
			{
				MarketServiceId = msId,
				EndTime = end,
				StartTime = start
			};
			marketServiceRecordInfo.Add(marketServiceRecordInfo1);
		}

		public void AddServiceCategory(string categoryName)
		{
			if (string.IsNullOrWhiteSpace(categoryName))
			{
				throw new HimallException("分类不能为空，添加失败.");
			}
			MarketSettingInfo marketSettingMetaInfos = (
				from m in context.MarketSettingInfo
				where (int)m.TypeId == 1
				select m).FirstOrDefault();
			if (marketSettingMetaInfos != null && marketSettingMetaInfos.Id != 0)
			{
				if (marketSettingMetaInfos.Himall_MarketSettingMeta.Count() == 0)
				{
					marketSettingMetaInfos.Himall_MarketSettingMeta = new List<MarketSettingMetaInfo>();
				}
				MarketSettingMetaInfo marketSettingMetaInfo = marketSettingMetaInfos.Himall_MarketSettingMeta.FirstOrDefault((MarketSettingMetaInfo m) => {
					if (m.MarketId != marketSettingMetaInfos.Id)
					{
						return false;
					}
					return m.MetaKey.ToLower().Equals("categories");
				});
				if (marketSettingMetaInfo == null || marketSettingMetaInfo.Id == 0)
				{
					ICollection<MarketSettingMetaInfo> himallMarketSettingMeta = marketSettingMetaInfos.Himall_MarketSettingMeta;
					MarketSettingMetaInfo marketSettingMetaInfo1 = new MarketSettingMetaInfo()
					{
						MetaKey = "Categories",
						MetaValue = categoryName,
						MarketId = marketSettingMetaInfos.Id
					};
					himallMarketSettingMeta.Add(marketSettingMetaInfo1);
				}
				else
				{
					string metaValue = marketSettingMetaInfo.MetaValue;
					char[] chrArray = new char[] { ',' };
					if (metaValue.Split(chrArray).Any((string c) => c.Equals(categoryName)))
					{
						throw new HimallException("添加的限时购分类已经存在，添加失败.");
					}
					MarketSettingMetaInfo marketSettingMetaInfo2 = marketSettingMetaInfo;
					marketSettingMetaInfo2.MetaValue = string.Concat(marketSettingMetaInfo2.MetaValue, string.Format(",{0}", categoryName));
				}
			}
            context.SaveChanges();
		}

		public void AuditItem(long Id, LimitTimeMarketInfo.LimitTimeMarketAuditStatus status, string message)
		{
			LimitTimeMarketInfo now = context.LimitTimeMarketInfo.FindById<LimitTimeMarketInfo>(Id);
			now.AuditStatus = status;
			now.CancelReson = message ?? "";
			now.AuditTime = DateTime.Now;
            context.SaveChanges();
		}

		private void CheckLimit(LimitTimeMarketInfo model)
		{
			if (context.LimitTimeMarketInfo.Any((LimitTimeMarketInfo m) => m.Id != model.Id && m.ShopId == model.ShopId && m.ProductId == model.ProductId && (m.EndTime > DateTime.Now) && ((int)m.AuditStatus == 2 || (int)m.AuditStatus == 1)))
			{
				throw new HimallException(string.Format("操作失败，限时购活动：{0} 已经存在.", model.ProductName));
			}
		}

		public void DeleteServiceCategory(string categoryName)
		{
			if (string.IsNullOrWhiteSpace(categoryName))
			{
				throw new HimallException("分类不能为空，添加失败.");
			}
			MarketSettingInfo marketSettingInfo = (
				from m in context.MarketSettingInfo
				where (int)m.TypeId == 1
				select m).FirstOrDefault();
			if (context.LimitTimeMarketInfo.Any((LimitTimeMarketInfo m) => m.CategoryName.Equals(categoryName)))
			{
				throw new HimallException("该分类不能被删除，因为其他限时购活动正在使用.");
			}
			if (marketSettingInfo != null && marketSettingInfo.Id != 0)
			{
				MarketSettingMetaInfo marketSettingMetaInfo = marketSettingInfo.Himall_MarketSettingMeta.FirstOrDefault((MarketSettingMetaInfo m) => {
					if (m.MarketId != marketSettingInfo.Id)
					{
						return false;
					}
					return m.MetaKey.ToLower().Equals("categories");
				});
				if (marketSettingMetaInfo != null && marketSettingMetaInfo.Id != 0 && !string.IsNullOrWhiteSpace(marketSettingMetaInfo.MetaValue))
				{
					string metaValue = marketSettingMetaInfo.MetaValue;
					char[] chrArray = new char[] { ',' };
					List<string> list = metaValue.Split(chrArray).ToList();
					list.Remove(categoryName);
					marketSettingMetaInfo.MetaValue = string.Join(",", list);
				}
			}
            context.SaveChanges();
		}

		public void EnableMarketService(int monthCount, long shopId)
		{
			if (shopId <= 0)
			{
				throw new HimallException("ShopId不能识别");
			}
			if (monthCount <= 0)
			{
				throw new HimallException("购买服务时长必须大于零");
			}
			ShopInfo shopInfo = context.ShopInfo.FindById<ShopInfo>(shopId);
			if (shopInfo == null || shopId <= 0)
			{
				throw new HimallException("ShopId不能识别");
			}
			if (!context.ActiveMarketServiceInfo.Any((ActiveMarketServiceInfo a) => a.ShopId == shopId && (int)a.TypeId == 1))
			{
				DbSet<ActiveMarketServiceInfo> activeMarketServiceInfo = context.ActiveMarketServiceInfo;
				ActiveMarketServiceInfo activeMarketServiceInfo1 = new ActiveMarketServiceInfo()
				{
					ShopId = shopId,
					ShopName = shopInfo.ShopName,
					TypeId = MarketType.LimitTimeBuy
				};
				List<MarketServiceRecordInfo> marketServiceRecordInfos = new List<MarketServiceRecordInfo>();
				MarketServiceRecordInfo marketServiceRecordInfo = new MarketServiceRecordInfo()
				{
					StartTime = DateTime.Now.Date,
					EndTime = DateTime.Now.Date.AddMonths(monthCount)
				};
				marketServiceRecordInfos.Add(marketServiceRecordInfo);
				activeMarketServiceInfo1.MarketServiceRecordInfo = marketServiceRecordInfos;
				activeMarketServiceInfo.Add(activeMarketServiceInfo1);
			}
			else
			{
				ActiveMarketServiceInfo activeMarketServiceInfo2 = context.ActiveMarketServiceInfo.FirstOrDefault((ActiveMarketServiceInfo a) => a.ShopId == shopId && (int)a.TypeId == 1);
				long id = activeMarketServiceInfo2.Id;
				DateTime date = DateTime.Now.Date;
				DateTime dateTime = DateTime.Now.Date;
                AddMarketServiceRecord(id, date, dateTime.AddMonths(monthCount));
			}
            context.SaveChanges();
		}

		public PageModel<ActiveMarketServiceInfo> GetBoughtShopList(MarketBoughtQuery query)
		{
			IQueryable<ActiveMarketServiceInfo> typeId = context.ActiveMarketServiceInfo.AsQueryable<ActiveMarketServiceInfo>();
			if (query.MarketType.HasValue)
			{
				typeId = 
					from d in typeId
					where (int?)d.TypeId == (int?)query.MarketType
					select d;
			}
			if (!string.IsNullOrWhiteSpace(query.ShopName))
			{
				typeId = 
					from d in typeId
					where d.ShopName.Contains(query.ShopName)
					select d;
			}
			int num = 0;
			typeId = typeId.GetPage(out num, (IQueryable<ActiveMarketServiceInfo> d) => 
				from o in d
				orderby o.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo m) => m.EndTime)
				select o, query.PageNo, query.PageSize);
			return new PageModel<ActiveMarketServiceInfo>()
			{
				Models = typeId,
				Total = num
			};
		}

		public PageModel<LimitTimeMarketInfo> GetItemList(LimitTimeQuery query)
		{
			IQueryable<LimitTimeMarketInfo> endTime = context.LimitTimeMarketInfo.AsQueryable<LimitTimeMarketInfo>();
			if (query.OrderType != 0)
			{
				endTime = 
					from item in endTime
					where item.EndTime > DateTime.Now
					select item;
			}
			if (query.ShopId.HasValue)
			{
				endTime = 
					from item in endTime
					where query.ShopId == item.ShopId
                    select item;
			}
			if (!string.IsNullOrWhiteSpace(query.ItemName))
			{
				endTime = 
					from item in endTime
					where item.ProductName.Contains(query.ItemName)
					select item;
			}
			if (!string.IsNullOrWhiteSpace(query.ShopName))
			{
				endTime = 
					from item in endTime
					where item.ShopName.Contains(query.ShopName)
					select item;
			}
			if (!string.IsNullOrWhiteSpace(query.CategoryName))
			{
				endTime = endTime.FindBy((LimitTimeMarketInfo d) => d.CategoryName == query.CategoryName);
			}
			DateTime now = DateTime.Now;
			if (query.AuditStatus.HasValue)
			{
				if (query.AuditStatus.Value == LimitTimeMarketInfo.LimitTimeMarketAuditStatus.Ended)
				{
					endTime = 
						from a in endTime
						where (a.EndTime < now) && ((int)a.AuditStatus == 2 || (int)a.AuditStatus == 1)
						select a;
				}
				else if (query.AuditStatus.Value != LimitTimeMarketInfo.LimitTimeMarketAuditStatus.Ongoing)
				{
					endTime = (query.AuditStatus.Value != LimitTimeMarketInfo.LimitTimeMarketAuditStatus.WaitForAuditing ? 
						from a in endTime
						where (int)a.AuditStatus == (int)query.AuditStatus.Value
						select a : 
						from a in endTime
						where (int)a.AuditStatus == 1 && (a.EndTime > now)
						select a);
				}
				else
				{
					endTime = 
						from a in endTime
						where (int)a.AuditStatus == 2 && (a.EndTime > now)
						select a;
				}
			}
			int num = 0;
			endTime = 
				from l in endTime
				join p in context.ProductInfo on l.ProductId equals p.Id
				where (int)p.AuditStatus == 2 && (!query.CheckProductStatus || query.CheckProductStatus && (int)p.SaleStatus == 1)
				select l;
			if (query.PageSize == 0)
			{
				query.PageSize = 10;
			}
			if (endTime.Count() / query.PageSize < query.PageNo - 1)
			{
				query.PageNo = 1;
			}
			Func<IQueryable<LimitTimeMarketInfo>, IOrderedQueryable<LimitTimeMarketInfo>> orderBy = endTime.GetOrderBy((IQueryable<LimitTimeMarketInfo> d) => 
				from item in d
				orderby item.Id descending
				select item);
			switch (query.OrderKey)
			{
				case 2:
				{
					orderBy = endTime.GetOrderBy((IQueryable<LimitTimeMarketInfo> d) => 
						from item in d
						orderby item.SaleCount
						select item);
					break;
				}
				case 3:
				{
					if (query.OrderType != 2)
					{
						orderBy = endTime.GetOrderBy((IQueryable<LimitTimeMarketInfo> d) => 
							from item in d
							orderby item.Price descending
							select item);
						break;
					}
					else
					{
						orderBy = endTime.GetOrderBy((IQueryable<LimitTimeMarketInfo> d) => 
							from item in d
							orderby item.Price
							select item);
						break;
					}
				}
				case 4:
				{
					orderBy = endTime.GetOrderBy((IQueryable<LimitTimeMarketInfo> d) => 
						from item in d
						orderby item.EndTime
						select item);
					break;
				}
				case 5:
				{
					orderBy = endTime.GetOrderBy((IQueryable<LimitTimeMarketInfo> d) => 
						from item in d
						orderby item.AuditStatus
						select item);
					break;
				}
				default:
				{
					orderBy = endTime.GetOrderBy((IQueryable<LimitTimeMarketInfo> d) => 
						from item in d
						orderby item.Id descending
						select item);
					break;
				}
			}
			endTime = endTime.GetPage(out num, orderBy, query.PageNo, query.PageSize);
			return new PageModel<LimitTimeMarketInfo>()
			{
				Models = endTime,
				Total = num
			};
		}

		public LimitTimeMarketInfo GetLimitTimeMarketItem(long id)
		{
			if (id <= 0)
			{
				throw new HimallException("限时购活动Id不能识别");
			}
			return context.LimitTimeMarketInfo.FindById<LimitTimeMarketInfo>(id);
		}

		public LimitTimeMarketInfo GetLimitTimeMarketItemByProductId(long pid)
		{
			if (pid <= 0)
			{
				throw new HimallException("商品Id不能识别");
			}
			return context.LimitTimeMarketInfo.FirstOrDefault((LimitTimeMarketInfo m) => m.ProductId == pid && (int)m.AuditStatus == 2 && (m.StartTime <= DateTime.Now) && (m.EndTime > DateTime.Now));
		}

		public int GetMarketSaleCountForUserId(long pId, long userId)
		{
			List<OrderItemInfo> list = (
				from a in context.OrderItemInfo.Include("OrderInfo")
				where a.ProductId == pId && a.IsLimitBuy && a.OrderInfo.UserId == userId && (int)a.OrderInfo.OrderStatus != 4
				select a).ToList();
			if (list.Count == 0)
			{
				return 0;
			}
			LimitTimeMarketInfo limitTimeMarketInfo = (
				from item in context.LimitTimeMarketInfo
				where item.ProductId == pId && (int)item.AuditStatus == 2 && (item.EndTime >= DateTime.Now)
				select item).FirstOrDefault();
			long? nullable = (
				from a in list
				where a.OrderInfo.OrderDate >= limitTimeMarketInfo.StartTime
				select a).Sum<OrderItemInfo>((OrderItemInfo a) => new long?(a.Quantity));
			return (int)nullable.GetValueOrDefault();
		}

		public ActiveMarketServiceInfo GetMarketService(long shopId)
		{
			if (shopId <= 0)
			{
				throw new HimallException("ShopId不能识别");
			}
			return context.ActiveMarketServiceInfo.FirstOrDefault((ActiveMarketServiceInfo m) => m.ShopId == shopId && (int)m.TypeId == 1);
		}

		public string[] GetServiceCategories()
		{
			List<string> strs = new List<string>();
			MarketSettingInfo marketSettingInfo = (
				from m in context.MarketSettingInfo
				where (int)m.TypeId == 1
				select m).FirstOrDefault();
			if (marketSettingInfo != null && marketSettingInfo.Id != 0)
			{
				MarketSettingMetaInfo marketSettingMetaInfo = marketSettingInfo.Himall_MarketSettingMeta.FirstOrDefault((MarketSettingMetaInfo m) => {
					if (m.MarketId != marketSettingInfo.Id)
					{
						return false;
					}
					return m.MetaKey.ToLower().Equals("categories");
				});
				if (marketSettingMetaInfo != null && marketSettingMetaInfo.Id != 0)
				{
					string metaValue = marketSettingMetaInfo.MetaValue;
					char[] chrArray = new char[] { ',' };
					strs = metaValue.Split(chrArray).ToList();
				}
			}
			return strs.ToArray();
		}

		public LimitTimeBuySettingModel GetServiceSetting()
		{
			int num = 0;
			MarketSettingInfo marketSettingInfo = (
				from m in context.MarketSettingInfo
				where (int)m.TypeId == 1
				select m).FirstOrDefault();
			if (marketSettingInfo == null || marketSettingInfo.Id == 0)
			{
				return null;
			}
			LimitTimeBuySettingModel limitTimeBuySettingModel = new LimitTimeBuySettingModel()
			{
				Price = marketSettingInfo.Price,
				ReviceDays = 0
			};
			LimitTimeBuySettingModel limitTimeBuySettingModel1 = limitTimeBuySettingModel;
			if (marketSettingInfo.Himall_MarketSettingMeta.Count() != 0)
			{
				MarketSettingMetaInfo marketSettingMetaInfo = marketSettingInfo.Himall_MarketSettingMeta.FirstOrDefault((MarketSettingMetaInfo m) => m.MetaKey.ToLower().Equals("revicedays"));
				if (marketSettingMetaInfo != null)
				{
					int.TryParse(marketSettingMetaInfo.MetaValue, out num);
				}
				limitTimeBuySettingModel1.ReviceDays = num;
			}
			return limitTimeBuySettingModel1;
		}

		public bool IsLimitTimeMarketItem(long id)
		{
			if (id <= 0)
			{
				return false;
			}
			return context.LimitTimeMarketInfo.Any((LimitTimeMarketInfo l) => l.ProductId == id && (int)l.AuditStatus == 2 && (l.StartTime <= DateTime.Now) && (l.EndTime > DateTime.Now));
		}

		public void UpdateLimitTimeItem(LimitTimeMarketInfo model)
		{
            CheckLimit(model);
			LimitTimeMarketInfo title = context.LimitTimeMarketInfo.FindById<LimitTimeMarketInfo>(model.Id);
			title.Title = model.Title;
			title.ProductId = model.ProductId;
			title.ProductName = model.ProductName;
			title.CategoryName = model.CategoryName;
			title.StartTime = model.StartTime;
			title.EndTime = model.EndTime;
			title.Price = model.Price;
			title.Stock = model.Stock;
			title.MaxSaleCount = model.MaxSaleCount;
            context.SaveChanges();
		}

		public void UpdateServiceSetting(LimitTimeBuySettingModel model)
		{
			MarketSettingInfo price = context.MarketSettingInfo.FirstOrDefault((MarketSettingInfo m) => (int)m.TypeId == 1);
			if (price == null || price.Id == 0)
			{
				MarketSettingInfo marketSettingInfo = new MarketSettingInfo()
				{
					Price = model.Price,
					TypeId = MarketType.LimitTimeBuy
				};
                context.MarketSettingInfo.Add(marketSettingInfo);
			}
			else
			{
				price.Price = model.Price;
                //if (price.Himall_MarketSettingMeta.Count() != 0)
                //{
                //    MarketSettingMetaInfo str = price.Himall_MarketSettingMeta.FirstOrDefault((MarketSettingMetaInfo m) =>
                //    {
                //        if (m.MarketId != price.Id)
                //        {
                //            return false;
                //        }
                //        return m.MetaKey.ToLower().Equals("revicedays");
                //    });
                //    str.MetaValue = model.ReviceDays.ToString();
                //}
                
			}
            context.SaveChanges();
		}
	}
}