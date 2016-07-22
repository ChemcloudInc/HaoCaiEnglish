using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace Himall.Service
{
	public class GiftsOrderService : ServiceBase, IGiftsOrderService, IService, IDisposable
	{
		private static object obj;

		static GiftsOrderService()
		{
			GiftsOrderService.obj = new object();
		}

		public GiftsOrderService()
		{
		}

		public void AutoConfirmOrder()
		{
			int num;
			try
			{
				SiteSettingsInfo siteSettings = Instance<ISiteSettingService>.Create.GetSiteSettings();
				if (siteSettings == null)
				{
					num = 7;
				}
				else
				{
					num = (siteSettings.NoReceivingTimeout == 0 ? 7 : siteSettings.NoReceivingTimeout);
				}
				DateTime dateTime = DateTime.Now.AddDays(-num);
				List<GiftOrderInfo> list = (
					from a in context.GiftOrderInfo
					where (a.ShippingDate < dateTime) && (int)a.OrderStatus == 3
					select a).ToList();
				foreach (GiftOrderInfo nullable in list)
				{
					nullable.OrderStatus = GiftOrderInfo.GiftOrderStatus.Finish;
					nullable.CloseReason = "完成过期未确认收货的礼品订单";
					nullable.FinishDate = new DateTime?(DateTime.Now);
				}
                context.SaveChanges();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Log.Error(string.Concat("AutoConfirmGiftOrder:", exception.Message, "/r/n", exception.StackTrace));
			}
		}

		public void CloseOrder(long id, string closeReason)
		{
		}

		public void ConfirmOrder(long id, long userId)
		{
			GiftOrderInfo nullable = context.GiftOrderInfo.FirstOrDefault((GiftOrderInfo a) => a.UserId == userId && a.Id == id && (int)a.OrderStatus == 3);
			if (nullable == null)
			{
				throw new HimallException("错误的订单编号，或订单状态不对！");
			}
			nullable.OrderStatus = GiftOrderInfo.GiftOrderStatus.Finish;
			nullable.FinishDate = new DateTime?(DateTime.Now);
            context.SaveChanges();
		}

		public GiftOrderInfo CreateOrder(GiftOrderModel model)
		{
			if (model.CurrentUser == null)
			{
				throw new HimallException("错误的用户信息");
			}
			if (model.ReceiveAddress == null)
			{
				throw new HimallException("错误的收货人信息");
			}
			GiftOrderInfo giftOrderInfo = new GiftOrderInfo()
			{
				Id = GenerateOrderNumber(),
				UserId = model.CurrentUser.Id,
				RegionId = new int?(model.ReceiveAddress.RegionId),
				ShipTo = model.ReceiveAddress.ShipTo,
				Address = model.ReceiveAddress.Address,
				RegionFullName = model.ReceiveAddress.RegionFullName,
				CellPhone = model.ReceiveAddress.Phone
			};
			string regionIdPath = model.ReceiveAddress.RegionIdPath;
			char[] chrArray = new char[] { ',' };
			giftOrderInfo.TopRegionId = new int?(int.Parse(regionIdPath.Split(chrArray)[0]));
			giftOrderInfo.UserRemark = model.UserRemark;
			GiftOrderInfo now = giftOrderInfo;
			using (TransactionScope transactionScope = new TransactionScope())
			{
				foreach (GiftOrderItemModel gift in model.Gifts)
				{
					if (gift.Counts < 1)
					{
						throw new HimallException("错误的兑换数量！");
					}
					GiftInfo stockQuantity = context.GiftInfo.FirstOrDefault((GiftInfo d) => d.Id == gift.GiftId);
					if (stockQuantity == null || stockQuantity.GetSalesStatus != GiftInfo.GiftSalesStatus.Normal)
					{
						throw new HimallException("礼品不存在或己失效！");
					}
					if (stockQuantity.StockQuantity < gift.Counts)
					{
						throw new HimallException("礼品库存不足！");
					}
					stockQuantity.StockQuantity = stockQuantity.StockQuantity - gift.Counts;
					GiftInfo realSales = stockQuantity;
					realSales.RealSales = realSales.RealSales + gift.Counts;
					GiftOrderItemInfo giftOrderItemInfo = new GiftOrderItemInfo()
					{
						GiftId = stockQuantity.Id,
						GiftName = stockQuantity.GiftName,
						GiftValue = stockQuantity.GiftValue,
						ImagePath = stockQuantity.ImagePath,
						OrderId = new long?(now.Id),
						Quantity = gift.Counts,
						SaleIntegral = new int?(stockQuantity.NeedIntegral)
					};
					now.Himall_GiftOrderItem.Add(giftOrderItemInfo);
				}
				now.TotalIntegral = now.Himall_GiftOrderItem.Sum<GiftOrderItemInfo>((GiftOrderItemInfo d) => {
					int quantity = d.Quantity;
					int? saleIntegral = d.SaleIntegral;
					if (!saleIntegral.HasValue)
					{
						return null;
					}
					return new int?(quantity * saleIntegral.GetValueOrDefault());
				});
				now.OrderStatus = GiftOrderInfo.GiftOrderStatus.WaitDelivery;
				now.OrderDate = DateTime.Now;
                context.GiftOrderInfo.Add(now);
                context.SaveChanges();
				UserMemberInfo userMemberInfo = context.UserMemberInfo.FirstOrDefault((UserMemberInfo d) => d.Id == model.CurrentUser.Id);
                DeductionIntegral(userMemberInfo, now.Id, now.TotalIntegral.Value);
				transactionScope.Complete();
			}
			return now;
		}

		private void DeductionIntegral(UserMemberInfo member, long Id, int integral)
		{
			if (integral == 0)
			{
				return;
			}
			MemberIntegralRecord memberIntegralRecord = new MemberIntegralRecord()
			{
				UserName = member.UserName,
				MemberId = member.Id,
				RecordDate = new DateTime?(DateTime.Now)
			};
			string str = "礼品订单号:";
			memberIntegralRecord.TypeId = MemberIntegral.IntegralType.Exchange;
			str = string.Concat(str, Id.ToString());
			MemberIntegralRecordAction memberIntegralRecordAction = new MemberIntegralRecordAction()
			{
				VirtualItemTypeId = new MemberIntegral.VirtualItemType?(MemberIntegral.VirtualItemType.Exchange),
				VirtualItemId = Id
			};
			memberIntegralRecord.Himall_MemberIntegralRecordAction.Add(memberIntegralRecordAction);
			memberIntegralRecord.ReMark = str;
			IConversionMemberIntegralBase conversionMemberIntegralBase = Instance<IMemberIntegralConversionFactoryService>.Create.Create(MemberIntegral.IntegralType.Exchange, integral);
			Instance<IMemberIntegralService>.Create.AddMemberIntegral(memberIntegralRecord, conversionMemberIntegralBase);
		}

		public long GenerateOrderNumber()
		{
			long num;
			lock (GiftsOrderService.obj)
			{
				string empty = string.Empty;
				Guid guid = Guid.NewGuid();
				Random random = new Random(BitConverter.ToInt32(guid.ToByteArray(), 0));
				for (int i = 0; i < 4; i++)
				{
					int num1 = random.Next();
					char chr = (char)(48 + (ushort)(num1 % 10));
					empty = string.Concat(empty, chr.ToString());
				}
				DateTime now = DateTime.Now;
				num = long.Parse(string.Concat("1", now.ToString("yyyyMMddfff"), empty));
			}
			return num;
		}

		public GiftOrderInfo GetOrder(long orderId)
		{
			return context.GiftOrderInfo.FirstOrDefault((GiftOrderInfo d) => d.Id == orderId);
		}

		public GiftOrderInfo GetOrder(long orderId, long userId)
		{
			return context.GiftOrderInfo.FirstOrDefault((GiftOrderInfo d) => d.Id == orderId && d.UserId == userId);
		}

		public GiftOrderItemInfo GetOrderItemById(long id)
		{
			return context.GiftOrderItemInfo.FirstOrDefault((GiftOrderItemInfo d) => d.Id == id);
		}

		public PageModel<GiftOrderInfo> GetOrders(GiftsOrderQuery query)
		{
			long num;
			PageModel<GiftOrderInfo> pageModel = new PageModel<GiftOrderInfo>();
			long.TryParse(query.skey, out num);
			IQueryable<GiftOrderInfo> id = context.GiftOrderInfo.AsQueryable<GiftOrderInfo>();
			if (!string.IsNullOrWhiteSpace(query.skey))
			{
				id = 
					from d in id
					where d.ShipTo.Contains(query.skey) || d.Id == num || d.Himall_GiftOrderItem.Any((GiftOrderItemInfo di) => di.GiftName.Contains(query.skey))
					select d;
			}
			if (query.OrderId.HasValue)
			{
				id = 
					from d in id
					where d.Id == query.OrderId.Value
					select d;
			}
			if (query.status.HasValue)
			{
				id = 
					from d in id
					where (int)d.OrderStatus == (int)query.status.Value
					select d;
			}
			if (query.UserId.HasValue)
			{
				id = 
					from d in id
					where d.UserId == query.UserId.Value
					select d;
			}
			Func<IQueryable<GiftOrderInfo>, IOrderedQueryable<GiftOrderInfo>> orderBy = id.GetOrderBy<GiftOrderInfo>((IQueryable<GiftOrderInfo> o) => 
				from d in o
				orderby d.Id descending
				select d);
			string sort = query.Sort;
			orderBy = id.GetOrderBy<GiftOrderInfo>((IQueryable<GiftOrderInfo> o) => 
				from d in o
				orderby d.OrderDate descending, d.Id descending
				select d);
			int num1 = 0;
			id = id.GetPage(out num1, query.PageNo, query.PageSize, orderBy);
			pageModel.Models = id;
			pageModel.Total = num1;
			return pageModel;
		}

		public IEnumerable<GiftOrderInfo> GetOrders(IEnumerable<long> ids)
		{
			List<GiftOrderInfo> list = (
				from d in (
					from d in context.GiftOrderInfo
					orderby d.Id descending
					select d).Include<GiftOrderInfo, ICollection<GiftOrderItemInfo>>((GiftOrderInfo d) => d.Himall_GiftOrderItem)
				where ids.Contains(d.Id)
				select d).ToList();
			return list;
		}

		public int GetOwnBuyQuantity(long userid, long giftid)
		{
			int num = 0;
			num = (
				from d in context.GiftOrderItemInfo
				where d.GiftId == giftid && d.Himall_GiftsOrder.UserId == userid
				select d).Count();
			return num;
		}

		public IEnumerable<GiftOrderInfo> OrderAddUserInfo(IEnumerable<GiftOrderInfo> orders)
		{
			if (orders.Count() > 0)
			{
				List<long> list = (
					from d in orders
					select d.UserId).ToList();
				if (list.Count > 0)
				{
					List<UserMemberInfo> userMemberInfos = (
						from d in context.UserMemberInfo
						where list.Contains(d.Id)
						select d).ToList();
					if (userMemberInfos.Count > 0)
					{
						foreach (GiftOrderInfo order in orders)
						{
							UserMemberInfo userMemberInfo = userMemberInfos.FirstOrDefault((UserMemberInfo d) => d.Id == order.UserId);
							order.UserName = (userMemberInfo == null ? "" : userMemberInfo.UserName);
						}
					}
				}
			}
			return orders;
		}

		public void SendGood(long id, string shipCompanyName, string shipOrderNumber)
		{
			GiftOrderInfo order = GetOrder(id);
			if (string.IsNullOrWhiteSpace(shipCompanyName) || string.IsNullOrWhiteSpace(shipOrderNumber))
			{
				throw new HimallException("请填写快递公司与快递单号");
			}
			if (order == null)
			{
				throw new HimallException("错误的订单编号");
			}
			if (order.OrderStatus != GiftOrderInfo.GiftOrderStatus.WaitDelivery)
			{
				throw new HimallException("订单状态有误，不可重复发货");
			}
			order.ExpressCompanyName = shipCompanyName;
			order.ShipOrderNumber = shipOrderNumber;
			order.ShippingDate = new DateTime?(DateTime.Now);
			order.OrderStatus = GiftOrderInfo.GiftOrderStatus.WaitReceiving;
            context.SaveChanges();
		}
	}
}