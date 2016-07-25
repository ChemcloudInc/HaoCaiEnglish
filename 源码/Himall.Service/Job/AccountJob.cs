using Himall.Core;
using Himall.Entity;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Service;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace Himall.Service.Job
{
	public class AccountJob : IJob
	{
		public AccountJob()
		{
		}

		private void CalculationMoney(DateTime startDate, DateTime endDate)
		{
			DateTime? finishDate;
			string[] str;
			Entities entity = new Entities();
			var list = (
				from p in entity.OrderInfo
				join o in entity.OrderRefundInfo on p.Id equals o.OrderId
				join x in entity.OrderItemInfo on o.OrderId equals x.OrderId
				where (int)p.OrderStatus == 5 && (o.ManagerConfirmDate >= startDate) && (o.ManagerConfirmDate < endDate) && (int)o.ManagerConfirmStatus == 7
				select new { Order = p, OrderRefund = o, OrderItem = x }).Distinct().ToList();
			var collection = (
				from p in entity.OrderInfo
				join o in entity.OrderItemInfo on p.Id equals o.OrderId
				where (int)p.OrderStatus == 5 && (p.FinishDate >= startDate) && (p.FinishDate < endDate)
				select new { Order = p, OrderItem = o }).ToList();
			List<long> nums = new List<long>();
			nums.AddRange(
				from c in list
				select c.Order.ShopId);
			nums.AddRange(
				from c in collection
				select c.Order.ShopId);
			nums = nums.Distinct<long>().ToList();
			using (TransactionScope transactionScope = new TransactionScope())
			{
				try
				{
					foreach (long num in nums)
					{
						List<OrderInfo> orderInfos = (
							from c in collection
							where c.Order.ShopId == num
							select c.Order).Distinct<OrderInfo>().ToList();
						decimal num1 = orderInfos.Sum<OrderInfo>((OrderInfo c) => c.ProductTotalAmount) - orderInfos.Sum<OrderInfo>((OrderInfo c) => c.DiscountAmount);
						decimal num2 = orderInfos.Sum<OrderInfo>((OrderInfo c) => c.Freight);
						decimal num3 = CalculationTotalCommission((
							from c in collection
							where c.Order.ShopId == num
							select c.OrderItem).Distinct<OrderItemInfo>().ToList());
						decimal num4 = CalculationTotalRefundCommission((
							from c in list
							where c.OrderRefund.ShopId == num
							select c.OrderItem).Distinct<OrderItemInfo>().ToList());
						decimal num5 = (
							from c in list
							where c.OrderRefund.ShopId == num
							select c.OrderRefund).Distinct<OrderRefundInfo>().Sum<OrderRefundInfo>((OrderRefundInfo c) => c.Amount);
						decimal num6 = (((num1 + num2) - num3) - num5) + num4;
						AccountInfo accountInfo = new AccountInfo()
						{
							ShopId = num,
							ShopName = (
								from c in entity.ShopInfo
								where c.Id == num
								select c).FirstOrDefault().ShopName,
							AccountDate = DateTime.Now,
							StartDate = startDate,
							EndDate = endDate.AddSeconds(-1),
							Status = AccountInfo.AccountStatus.UnAccount,
							ProductActualPaidAmount = num1,
							FreightAmount = num2,
							CommissionAmount = num3,
							RefundCommissionAmount = num4,
							RefundAmount = num5,
							PeriodSettlement = num6,
							Remark = string.Empty
						};
						entity.AccountInfo.Add(accountInfo);
						foreach (OrderInfo orderInfo in (
							from c in list
							where c.Order.ShopId == num
							select c.Order).Distinct<OrderInfo>().ToList())
						{
							AccountDetailInfo accountDetailInfo = new AccountDetailInfo()
							{
								Himall_Accounts = accountInfo,
								ShopId = orderInfo.ShopId
							};
							finishDate = orderInfo.FinishDate;
							accountDetailInfo.Date = finishDate.Value;
							accountDetailInfo.OrderType = AccountDetailInfo.EnumOrderType.ReturnOrder;
							accountDetailInfo.OrderId = orderInfo.Id;
							accountDetailInfo.ProductActualPaidAmount = orderInfo.ProductTotalAmount - orderInfo.DiscountAmount;
							accountDetailInfo.FreightAmount = orderInfo.Freight;
							accountDetailInfo.CommissionAmount = CalculationTotalCommission((
								from c in list
								where c.OrderRefund.OrderId == orderInfo.Id
								select c.OrderItem).Distinct<OrderItemInfo>().ToList());
							accountDetailInfo.RefundCommisAmount = CalculationTotalRefundCommission((
								from c in list
								where c.OrderRefund.OrderId == orderInfo.Id
								select c.OrderItem).Distinct<OrderItemInfo>().ToList());
							accountDetailInfo.RefundTotalAmount = (
								from c in list
								where c.OrderRefund.OrderId == orderInfo.Id
								select c.OrderRefund).Distinct<OrderRefundInfo>().Sum<OrderRefundInfo>((OrderRefundInfo c) => c.Amount);
							accountDetailInfo.OrderDate = orderInfo.OrderDate;
							accountDetailInfo.OrderRefundsDates = string.Join<DateTime>(";", (
								from c in list
								where c.OrderRefund.OrderId == orderInfo.Id
								select c.OrderRefund.ManagerConfirmDate).Distinct<DateTime>());
							entity.AccountDetailInfo.Add(accountDetailInfo);
						}
						foreach (OrderInfo orderInfo1 in orderInfos)
						{
							AccountDetailInfo value = new AccountDetailInfo()
							{
								Himall_Accounts = accountInfo,
								ShopId = orderInfo1.ShopId
							};
							finishDate = orderInfo1.FinishDate;
							value.Date = finishDate.Value;
							value.OrderType = AccountDetailInfo.EnumOrderType.FinishedOrder;
							value.OrderId = orderInfo1.Id;
							value.ProductActualPaidAmount = orderInfo1.ProductTotalAmount - orderInfo1.DiscountAmount;
							value.FreightAmount = orderInfo1.Freight;
							value.CommissionAmount = CalculationTotalCommission((
								from c in collection
								where c.Order.Id == orderInfo1.Id
								select c.OrderItem).Distinct<OrderItemInfo>().ToList());
							value.RefundCommisAmount = new decimal(0);
							value.RefundTotalAmount = new decimal(0);
							value.OrderDate = orderInfo1.OrderDate;
							value.OrderRefundsDates = string.Empty;
							entity.AccountDetailInfo.Add(value);
						}
					}
					entity.SaveChanges();
					transactionScope.Complete();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					str = new string[] { "CalculationMoney ：startDate=", startDate.ToString(), " endDate=", endDate.ToString(), "/r/n", exception.Message };
					Log.Error(string.Concat(str));
				}
			}
			try
			{
				var list1 = (
					from b in entity.ActiveMarketServiceInfo
					join c in entity.MarketServiceRecordInfo on b.Id equals c.MarketServiceId
					join d in nums on b.ShopId equals d
					join aa in entity.MarketSettingInfo on b.TypeId equals aa.TypeId
					where c.SettlementFlag == 0
                    select new { ShopId = b.ShopId, TypeId = b.TypeId, Price = ((c.EndTime.Year * 12 + c.EndTime.Month - (c.StartTime.Year * 12 + c.StartTime.Month)) * aa.Price), MSRecordId = c.Id, StartTime = c.StartTime, EndTime = c.EndTime }).ToList();
				List<AccountInfo> accountInfos = (
					from e in entity.AccountInfo
					where e.StartDate == startDate
					select e).ToList();
				IEnumerable<long> nums1 = (
					from e in list1
					select e.ShopId).Distinct<long>();
				foreach (AccountInfo periodSettlement in accountInfos)
				{
					var shopId = 
						from e in list1
						where e.ShopId == periodSettlement.ShopId
						select e;
					foreach (var variable in shopId)
					{
						periodSettlement.PeriodSettlement = periodSettlement.PeriodSettlement - variable.Price;
						periodSettlement.AdvancePaymentAmount = periodSettlement.AdvancePaymentAmount + variable.Price;
						DbSet<AccountMetaInfo> accountMetaInfo = entity.AccountMetaInfo;
						AccountMetaInfo accountMetaInfo1 = new AccountMetaInfo()
						{
							AccountId = periodSettlement.Id,
							MetaKey = variable.TypeId.ToDescription(),
							MetaValue = variable.Price.ToString("f2"),
							ServiceStartTime = variable.StartTime,
							ServiceEndTime = variable.EndTime
						};
						accountMetaInfo.Add(accountMetaInfo1);
					}
				}
				var collection1 = (
					from a in entity.MarketServiceRecordInfo
					join b in entity.ActiveMarketServiceInfo on a.MarketServiceId equals b.Id
					join c in nums1 on b.ShopId equals c
					where a.SettlementFlag == 0
                    select new { Shopid = b.ShopId, TypeId = b.TypeId, Msrecordid = a.Id }).ToList();
				foreach (var variable1 in collection1)
				{
					MarketServiceRecordInfo marketServiceRecordInfo = entity.MarketServiceRecordInfo.FirstOrDefault((MarketServiceRecordInfo e) => e.Id == variable1.Msrecordid);
					marketServiceRecordInfo.SettlementFlag = 1;
				}
				entity.SaveChanges();
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				str = new string[] { "CalculationMoney 服务费：startDate=", startDate.ToString(), " endDate=", endDate.ToString(), "/r/n", exception2.Message };
				Log.Error(string.Concat(str));
			}
		}

		private decimal CalculationTotalCommission(IList<OrderItemInfo> orderItems)
		{
			decimal num = new decimal(0);
			return orderItems.Sum<OrderItemInfo>((OrderItemInfo c) => c.RealTotalPrice * c.CommisRate);
		}

		private decimal CalculationTotalRefundCommission(IList<OrderItemInfo> orderItems)
		{
			decimal num = new decimal(0);
			return orderItems.Sum<OrderItemInfo>((OrderItemInfo c) => c.RefundPrice * c.CommisRate);
		}

		public void Execute(IJobExecutionContext context)
		{
			Entities entity = new Entities();
			DateTime minValue = DateTime.MinValue;
			SiteSettingsInfo siteSettings = (new SiteSettingService()).GetSiteSettings();
			AccountService accountService = new AccountService();
			AccountQuery accountQuery = new AccountQuery()
			{
				PageNo = 1,
				PageSize = 2147483647
			};
			AccountInfo accountInfo = (
				from c in accountService.GetAccounts(accountQuery).Models
				orderby c.Id descending
				select c).FirstOrDefault();
			if (accountInfo != null)
			{
				DateTime date = accountInfo.StartDate.Date;
				minValue = date.AddDays(siteSettings.WeekSettlement);
			}
			else
			{
				minValue = AccountJob.GetCheckDate(entity, minValue).Date;
			}
			if (minValue.Equals(DateTime.MinValue))
			{
				return;
			}
			while (minValue <= DateTime.Now.Date)
			{
				DateTime dateTime = minValue.Date;
				DateTime dateTime1 = dateTime.AddDays(siteSettings.WeekSettlement);
                CalculationMoney(dateTime, dateTime1);
				accountInfo = (
					from c in entity.AccountInfo
					where c.StartDate >= dateTime
					orderby c.EndDate descending
					select c).FirstOrDefault();
				if (accountInfo == null)
				{
					minValue = dateTime.AddDays(siteSettings.WeekSettlement);
				}
				else
				{
					DateTime date1 = accountInfo.StartDate.Date;
					minValue = date1.AddDays(siteSettings.WeekSettlement);
				}
			}
		}

		private static DateTime GetCheckDate(Entities entity, DateTime checkDate)
		{
			OrderInfo firstFinishedOrderForSettlement = (new OrderService()).GetFirstFinishedOrderForSettlement();
			if (firstFinishedOrderForSettlement != null)
			{
				checkDate = firstFinishedOrderForSettlement.FinishDate.Value;
			}
			else if (firstFinishedOrderForSettlement == null)
			{
				checkDate = DateTime.MinValue;
			}
			else
			{
				checkDate = firstFinishedOrderForSettlement.FinishDate.Value;
			}
			return checkDate;
		}

		private int GetMonths(DateTime dt1, DateTime dt2)
		{
			return dt2.Year * 12 + dt2.Month - (dt1.Year * 12 + dt1.Month);
		}
	}
}