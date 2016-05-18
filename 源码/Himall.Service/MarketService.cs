using Himall.Core;
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
	public class MarketService : ServiceBase, IMarketService, IService, IDisposable
	{
		public MarketService()
		{
		}

		public void AddOrUpdateServiceSetting(MarketSettingInfo info)
		{
			MarketSettingInfo price = context.MarketSettingInfo.FirstOrDefault((MarketSettingInfo a) => (int)a.TypeId == (int)info.TypeId);
			if (price != null)
			{
				price.Price = info.Price;
			}
			else
			{
                context.MarketSettingInfo.Add(info);
			}
            context.SaveChanges();
		}

		public PageModel<MarketServiceRecordInfo> GetBoughtShopList(MarketBoughtQuery query)
		{
			IQueryable<MarketServiceRecordInfo> typeId = context.MarketServiceRecordInfo.AsQueryable<MarketServiceRecordInfo>();
			if (query.MarketType.HasValue)
			{
				typeId = 
					from d in typeId
					where (int)d.ActiveMarketServiceInfo.TypeId == (int)query.MarketType.Value
					select d;
			}
			if (!string.IsNullOrWhiteSpace(query.ShopName))
			{
				typeId = 
					from d in typeId
					where d.ActiveMarketServiceInfo.ShopName.Contains(query.ShopName)
					select d;
			}
			int num = 0;
			typeId = typeId.GetPage(out num, query.PageNo, query.PageSize, null);
			return new PageModel<MarketServiceRecordInfo>()
			{
				Models = typeId,
				Total = num
			};
		}

		public ActiveMarketServiceInfo GetMarketService(long shopId, MarketType type)
		{
			return context.ActiveMarketServiceInfo.FirstOrDefault((ActiveMarketServiceInfo m) => (int)m.TypeId == (int)type && m.ShopId == shopId);
		}

		public MarketSettingInfo GetServiceSetting(MarketType type)
		{
			return context.MarketSettingInfo.FirstOrDefault((MarketSettingInfo m) => (int)m.TypeId == (int)type);
		}

		public void OrderMarketService(int monthCount, long shopId, MarketType type)
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
			ActiveMarketServiceInfo activeMarketServiceInfo = context.ActiveMarketServiceInfo.FirstOrDefault((ActiveMarketServiceInfo a) => a.ShopId == shopId && (int)a.TypeId == (int)type);
			if (activeMarketServiceInfo == null)
			{
				DbSet<ActiveMarketServiceInfo> activeMarketServiceInfos = context.ActiveMarketServiceInfo;
				ActiveMarketServiceInfo activeMarketServiceInfo1 = new ActiveMarketServiceInfo()
				{
					ShopId = shopId,
					ShopName = shopInfo.ShopName,
					TypeId = type
				};
				List<MarketServiceRecordInfo> marketServiceRecordInfos = new List<MarketServiceRecordInfo>();
				MarketServiceRecordInfo marketServiceRecordInfo = new MarketServiceRecordInfo()
				{
					StartTime = DateTime.Now.Date,
					EndTime = DateTime.Now.AddMonths(monthCount)
				};
				marketServiceRecordInfos.Add(marketServiceRecordInfo);
				activeMarketServiceInfo1.MarketServiceRecordInfo = marketServiceRecordInfos;
				activeMarketServiceInfos.Add(activeMarketServiceInfo1);
			}
			else
			{
				DateTime dateTime = activeMarketServiceInfo.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo item) => item.EndTime);
				ICollection<MarketServiceRecordInfo> marketServiceRecordInfo1 = activeMarketServiceInfo.MarketServiceRecordInfo;
				MarketServiceRecordInfo marketServiceRecordInfo2 = new MarketServiceRecordInfo()
				{
					StartTime = dateTime,
					EndTime = dateTime.AddMonths(monthCount)
				};
				marketServiceRecordInfo1.Add(marketServiceRecordInfo2);
			}
            context.SaveChanges();
		}
	}
}