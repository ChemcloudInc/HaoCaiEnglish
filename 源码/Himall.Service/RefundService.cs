using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.Message;
using Himall.Core.Plugins.Payment;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Service.Order.Business;
using Himall.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Himall.Service
{
	public class RefundService : ServiceBase, IRefundService, IService, IDisposable
	{
		public RefundService()
		{
		}

		public void AddOrderRefund(OrderRefundInfo info)
		{
			bool flag = false;
			if (info.RefundMode == OrderRefundInfo.OrderRefundMode.OrderRefund)
			{
				flag = true;
			}
			bool flag1 = false;
			flag1 = (!flag ? CanApplyRefund(info.OrderId, info.OrderItemId, new bool?(false)) : CanApplyRefund(info.OrderId, info.OrderItemId, new bool?(true)));
			if (!flag1)
			{
				throw new HimallException("您己申请过售后，不可重复申请");
			}
			if (!flag)
			{
				if (info.ReturnQuantity <= 0)
				{
					info.RefundMode = OrderRefundInfo.OrderRefundMode.OrderItemRefund;
				}
				else
				{
					info.RefundMode = OrderRefundInfo.OrderRefundMode.ReturnGoodsRefund;
				}
			}
			info.SellerAuditDate = DateTime.Now;
			info.SellerAuditStatus = OrderRefundInfo.OrderRefundAuditStatus.WaitAudit;
			info.ManagerConfirmDate = DateTime.Now;
			info.ManagerConfirmStatus = OrderRefundInfo.OrderRefundConfirmStatus.UnConfirm;
			if (flag)
			{
				info.OrderItemId = context.OrderItemInfo.FirstOrDefault((OrderItemInfo d) => d.OrderId == info.OrderId).Id;
			}
			List<OrderItemInfo> orderItemInfos = new List<OrderItemInfo>();
			if (flag)
			{
				orderItemInfos = (
					from d in context.OrderItemInfo
					where d.OrderId == info.OrderId
					select d).ToList();
				foreach (OrderItemInfo orderItemInfo in orderItemInfos)
				{
					orderItemInfo.ReturnQuantity = orderItemInfo.Quantity;
					if (!orderItemInfo.EnabledRefundAmount.HasValue)
					{
						orderItemInfo.EnabledRefundAmount = new decimal?(new decimal(0));
					}
					orderItemInfo.RefundPrice = orderItemInfo.EnabledRefundAmount.Value;
				}
				info.ReturnQuantity = (int)orderItemInfos.Sum<OrderItemInfo>((OrderItemInfo d) => d.Quantity);
			}
			else
			{
				OrderItemInfo returnQuantity = context.OrderItemInfo.FirstOrDefault((OrderItemInfo d) => d.Id == info.OrderItemId);
				if (returnQuantity.Quantity - returnQuantity.ReturnQuantity < info.ReturnQuantity || (returnQuantity.RealTotalPrice - returnQuantity.RefundPrice) < info.Amount)
				{
					throw new HimallException("退货和退款数量不能超过订单的实际数量和金额！");
				}
				returnQuantity.ReturnQuantity = info.ReturnQuantity;
				returnQuantity.RefundPrice = info.Amount;
			}
            context.OrderRefundInfo.Add(info);
            context.SaveChanges();
		}

		public bool CanApplyRefund(long orderId, long orderItemId, bool? isAllOrderRefund = null)
		{
			IQueryable<OrderRefundInfo> orderRefundInfo = 
				from d in context.OrderRefundInfo
				where d.OrderId == orderId
				select d;
			bool? nullable = isAllOrderRefund;
			if ((!nullable.GetValueOrDefault() ? true : !nullable.HasValue))
			{
				orderRefundInfo = 
					from d in orderRefundInfo
					where d.OrderItemId == orderItemId
					select d;
				bool? nullable1 = isAllOrderRefund;
				if ((nullable1.GetValueOrDefault() ? false : nullable1.HasValue))
				{
					orderRefundInfo = 
						from d in orderRefundInfo
						where (int)d.RefundMode != 1
						select d;
				}
			}
			else
			{
				orderRefundInfo = 
					from d in orderRefundInfo
					where (int)d.RefundMode == 1
					select d;
			}
			return orderRefundInfo.Count() < 1;
		}

		public void ConfirmRefund(long refundId, string managerRemark, string managerName)
		{
			decimal? nullable;
			OrderRefundInfo now = context.OrderRefundInfo.FindById<OrderRefundInfo>(refundId);
			if (now.RefundPayType.HasValue)
			{
				switch (now.RefundPayType.Value)
				{
					case OrderRefundInfo.OrderRefundPayType.BackOut:
					{
						if (now.RefundPayStatus.HasValue && now.RefundPayStatus.Value == OrderRefundInfo.OrderRefundPayStatus.PaySuccess)
						{
							break;
						}
						string paymentTypeGateway = now.OrderItemInfo.OrderInfo.PaymentTypeGateway;
						IEnumerable<Plugin<IPaymentPlugin>> plugins = 
							from item in PluginsManagement.GetPlugins<IPaymentPlugin>(true)
							where item.PluginInfo.PluginId == paymentTypeGateway
							select item;
						if (plugins.Count<Plugin<IPaymentPlugin>>() <= 0)
						{
							throw new HimallException("退款时，未找到支付方式！");
						}
						OrderPayInfo orderPayInfo = context.OrderPayInfo.FirstOrDefault((OrderPayInfo e) => e.PayState && e.OrderId == now.OrderId);
						IQueryable<long> nums = 
							from item in context.OrderPayInfo
							where item.PayId == orderPayInfo.PayId && item.PayState
							select item into e
							select e.OrderId;
						decimal num = (
							from o in context.OrderInfo
							where nums.Contains(o.Id)
							select o).ToList().Sum<OrderInfo>((OrderInfo e) => e.OrderTotalAmount);
						if (orderPayInfo == null)
						{
							throw new HimallException("退款时，未找到原支付订单信息！");
						}
						PaymentPara paymentPara = new PaymentPara()
						{
							out_refund_no = now.Id.ToString(),
							out_trade_no = orderPayInfo.PayId.ToString(),
							refund_fee = now.Amount,
							total_fee = num
						};
						PaymentPara paymentPara1 = paymentPara;
						PaymentInfo paymentInfo = plugins.FirstOrDefault<Plugin<IPaymentPlugin>>().Biz.ProcessRefundFee(paymentPara1);
						if (paymentInfo.OrderIds == null || paymentInfo.OrderIds.Count() <= 0)
						{
							break;
						}
						now.RefundPayStatus = new OrderRefundInfo.OrderRefundPayStatus?(OrderRefundInfo.OrderRefundPayStatus.PaySuccess);
                            context.SaveChanges();
						break;
					}
					case OrderRefundInfo.OrderRefundPayType.BackCapital:
					{
						if (now.RefundPayStatus.HasValue && now.RefundPayStatus.Value == OrderRefundInfo.OrderRefundPayStatus.PaySuccess)
						{
							break;
						}
						CapitalInfo capitalInfo = context.CapitalInfo.FirstOrDefault((CapitalInfo e) => e.MemId == now.UserId);
						OrderBO orderBO = new OrderBO();
						if (capitalInfo != null)
						{
							CapitalDetailInfo capitalDetailInfo = new CapitalDetailInfo()
							{
								Amount = now.Amount,
								CapitalID = capitalInfo.Id,
								CreateTime = new DateTime?(DateTime.Now),
								SourceType = CapitalDetailInfo.CapitalDetailType.Refund,
								SourceData = now.Id.ToString(),
								Id = orderBO.GenerateOrderNumber()
							};
							CapitalDetailInfo capitalDetailInfo1 = capitalDetailInfo;
							CapitalInfo capitalInfo1 = capitalInfo;
							decimal? balance = capitalInfo1.Balance;
							decimal amount = now.Amount;
							if (balance.HasValue)
							{
								nullable = new decimal?(balance.GetValueOrDefault() + amount);
							}
							else
							{
								nullable = null;
							}
							capitalInfo1.Balance = nullable;
                                context.CapitalDetailInfo.Add(capitalDetailInfo1);
						}
						else
						{
							CapitalInfo capitalInfo2 = new CapitalInfo()
							{
								Balance = new decimal?(now.Amount),
								MemId = now.UserId,
								FreezeAmount = new decimal?(new decimal(0)),
								ChargeAmount = new decimal?(new decimal(0))
							};
							List<CapitalDetailInfo> capitalDetailInfos = new List<CapitalDetailInfo>();
							CapitalDetailInfo capitalDetailInfo2 = new CapitalDetailInfo()
							{
								Amount = now.Amount,
								CreateTime = new DateTime?(DateTime.Now),
								SourceData = now.Id.ToString(),
								SourceType = CapitalDetailInfo.CapitalDetailType.Refund,
								Id = orderBO.GenerateOrderNumber()
							};
							capitalDetailInfos.Add(capitalDetailInfo2);
							capitalInfo2.Himall_CapitalDetail = capitalDetailInfos;
							capitalInfo = capitalInfo2;
                                context.CapitalInfo.Add(capitalInfo);
						}
						now.RefundPayStatus = new OrderRefundInfo.OrderRefundPayStatus?(OrderRefundInfo.OrderRefundPayStatus.PaySuccess);
                            context.SaveChanges();
						break;
					}
				}
			}
			if (now.ManagerConfirmStatus != OrderRefundInfo.OrderRefundConfirmStatus.UnConfirm)
			{
				throw new HimallException("只有未确认状态的退款/退货才能进行确认操作");
			}
			now.ManagerConfirmStatus = OrderRefundInfo.OrderRefundConfirmStatus.Confirmed;
			now.ManagerConfirmDate = DateTime.Now;
			now.ManagerRemark = managerRemark;
			OrderOperationLogInfo orderOperationLogInfo = new OrderOperationLogInfo()
			{
				Operator = managerName,
				OrderId = now.OrderId,
				OperateDate = DateTime.Now,
				OperateContent = "确认退款/退货"
			};
            context.OrderOperationLogInfo.Add(orderOperationLogInfo);
			UserMemberInfo userMemberInfo = context.UserMemberInfo.FindById<UserMemberInfo>(now.UserId);
			OrderInfo orderInfo = context.OrderInfo.FindById<OrderInfo>(now.OrderId);
			decimal orderTotalAmount = orderInfo.OrderTotalAmount - orderInfo.Freight;
			decimal amount1 = now.Amount - (orderInfo.IntegralDiscount * (now.Amount / orderTotalAmount));
			amount1 = Math.Round(amount1, 2);
			if (amount1 > new decimal(0))
			{
				OrderInfo refundTotalAmount = orderInfo;
				refundTotalAmount.RefundTotalAmount = refundTotalAmount.RefundTotalAmount + amount1;
				if (orderInfo.RefundTotalAmount > orderTotalAmount)
				{
					orderInfo.RefundTotalAmount = orderTotalAmount;
				}
			}
			if (now.RefundMode != OrderRefundInfo.OrderRefundMode.OrderRefund)
			{
				MemberIntegralExchangeRules integralChangeRule = Instance<IMemberIntegralService>.Create.GetIntegralChangeRule();
				if (integralChangeRule != null)
				{
					int moneyPerIntegral = integralChangeRule.MoneyPerIntegral;
					int num1 = (int)Math.Floor(now.Amount / moneyPerIntegral);
					MemberIntegral memberIntegral = userMemberInfo.Himall_MemberIntegral.FirstOrDefault();
					int num2 = (memberIntegral == null ? 0 : memberIntegral.AvailableIntegrals);
					if (num1 > 0 && num2 > 0 && orderInfo.OrderStatus == OrderInfo.OrderOperateStatus.Finish)
					{
						MemberIntegralRecord memberIntegralRecord = new MemberIntegralRecord()
						{
							UserName = userMemberInfo.UserName,
							MemberId = userMemberInfo.Id,
							RecordDate = new DateTime?(DateTime.Now),
							TypeId = MemberIntegral.IntegralType.Others
						};
						object[] id = new object[] { "售后编号【", now.Id, "】退款应扣除积分", num1.ToString() };
						memberIntegralRecord.ReMark = string.Concat(id);
						num1 = (num1 > num2 ? num2 : num1);
						num1 = -num1;
						IConversionMemberIntegralBase conversionMemberIntegralBase = Instance<IMemberIntegralConversionFactoryService>.Create.Create(MemberIntegral.IntegralType.Others, num1);
						Instance<IMemberIntegralService>.Create.AddMemberIntegral(memberIntegralRecord, conversionMemberIntegralBase);
					}
				}
			}
            context.SaveChanges();
			MessageOrderInfo messageOrderInfo = new MessageOrderInfo()
			{
				OrderId = orderInfo.Id.ToString(),
				ShopId = orderInfo.ShopId,
				ShopName = orderInfo.ShopName,
				RefundMoney = now.Amount,
				SiteName = Instance<ISiteSettingService>.Create.GetSiteSettings().SiteName,
				TotalMoney = orderInfo.OrderTotalAmount
			};
			Task.Factory.StartNew(() => Instance<IMessageService>.Create.SendMessageOnOrderRefund(orderInfo.UserId, messageOrderInfo));
			if (orderInfo.PayDate.HasValue)
			{
                UpdateShopVisti(now, orderInfo.PayDate.Value);
                UpdateProductVisti(now, orderInfo.PayDate.Value);
			}
		}

		public IQueryable<OrderRefundInfo> GetAllOrderRefunds()
		{
			return context.OrderRefundInfo.FindAll<OrderRefundInfo>();
		}

		public OrderRefundInfo GetOrderRefund(long id, long userId)
		{
			return context.OrderRefundInfo.FirstOrDefault((OrderRefundInfo a) => a.Id == id && a.UserId == userId);
		}

		public PageModel<OrderRefundInfo> GetOrderRefunds(RefundQuery refundQuery)
		{
			int num;
			int value = 0;
			if (refundQuery.Status.HasValue)
			{
				value = refundQuery.Status.Value;
			}
			IQueryable<OrderRefundInfo> startDate = context.OrderRefundInfo.Include("OrderItemInfo").AsQueryable<OrderRefundInfo>();
			if (refundQuery.StartDate.HasValue)
			{
				startDate = 
					from item in startDate
					where refundQuery.StartDate <= item.ApplyDate
                    select item;
			}
			if (refundQuery.EndDate.HasValue)
			{
				startDate = 
					from item in startDate
					where refundQuery.EndDate >= item.ApplyDate
                    select item;
			}
			if (refundQuery.ConfirmStatus.HasValue)
			{
				startDate = 
					from item in startDate
					where (int?)refundQuery.ConfirmStatus == (int?)item.ManagerConfirmStatus
					select item;
			}
			if (refundQuery.ShopId.HasValue)
			{
				startDate = 
					from item in startDate
					where refundQuery.ShopId == item.ShopId
                    select item;
			}
			if (refundQuery.UserId.HasValue)
			{
				startDate = 
					from item in startDate
					where item.UserId == refundQuery.UserId
                    select item;
			}
			if (!string.IsNullOrWhiteSpace(refundQuery.ProductName))
			{
				startDate = 
					from item in startDate
					where item.OrderItemInfo.ProductName.Contains(refundQuery.ProductName)
					select item;
			}
			if (!string.IsNullOrWhiteSpace(refundQuery.ShopName))
			{
				startDate = 
					from item in startDate
					where item.ShopName.Contains(refundQuery.ShopName)
					select item;
			}
			if (!string.IsNullOrWhiteSpace(refundQuery.UserName))
			{
				startDate = 
					from item in startDate
					where item.Applicant.Contains(refundQuery.UserName)
					select item;
			}
			if (refundQuery.MoreOrderId == null)
			{
				refundQuery.MoreOrderId = new List<long>();
			}
			if (refundQuery.OrderId.HasValue)
			{
				refundQuery.MoreOrderId.Add(refundQuery.OrderId.Value);
				refundQuery.MoreOrderId = refundQuery.MoreOrderId.Distinct<long>().ToList();
				startDate = startDate.FindBy((OrderRefundInfo d) => refundQuery.MoreOrderId.Contains(d.OrderId));
			}
			if (refundQuery.ShowRefundType.HasValue)
			{
				int? showRefundType = refundQuery.ShowRefundType;
				int valueOrDefault = showRefundType.GetValueOrDefault();
				if (showRefundType.HasValue)
				{
					switch (valueOrDefault)
					{
						case 1:
						{
							startDate = startDate.FindBy((OrderRefundInfo d) => (int)d.RefundMode == 1);
							break;
						}
						case 2:
						{
							startDate = startDate.FindBy((OrderRefundInfo d) => (int)d.RefundMode == 2 || (int)d.RefundMode == 1);
							break;
						}
						case 3:
						{
							startDate = startDate.FindBy((OrderRefundInfo d) => (int)d.RefundMode == 3);
							break;
						}
						case 4:
						{
							startDate = startDate.FindBy((OrderRefundInfo d) => (int)d.RefundMode == 2);
							break;
						}
					}
				}
			}
			if (value > 0 && value <= 5)
			{
				startDate = 
					from item in startDate
					where (int)item.SellerAuditStatus == (int)value
                    select item;
			}
			if (value >= 6)
			{
				startDate = 
					from item in startDate
					where (int)item.ManagerConfirmStatus == (int)value && (int)item.SellerAuditStatus == 5
					select item;
			}
			if (refundQuery.AuditStatus.HasValue)
			{
				OrderRefundInfo.OrderRefundAuditStatus? auditStatus = refundQuery.AuditStatus;
				startDate = ((auditStatus.GetValueOrDefault() != OrderRefundInfo.OrderRefundAuditStatus.WaitAudit ? true : !auditStatus.HasValue) ? 
					from item in startDate
					where (int?)item.SellerAuditStatus == (int?)refundQuery.AuditStatus
					select item : 
					from item in startDate
					where (int)item.SellerAuditStatus == 1 || (int)item.SellerAuditStatus == 3
					select item);
			}
			startDate = startDate.GetPage(out num, refundQuery.PageNo, refundQuery.PageSize, null);
			List<OrderRefundInfo> list = startDate.ToList();
			List<long> nums = (
				from r in list
				select r.OrderId).ToList();
			List<OrderInfo> orderInfos = (
				from d in context.OrderInfo
				where nums.Contains(d.Id)
				select d).ToList();
			foreach (OrderRefundInfo productTotalAmount in list)
			{
				if (productTotalAmount.RefundMode != OrderRefundInfo.OrderRefundMode.OrderRefund)
				{
					productTotalAmount.EnabledRefundAmount = (!productTotalAmount.OrderItemInfo.EnabledRefundAmount.HasValue ? new decimal(0) : productTotalAmount.OrderItemInfo.EnabledRefundAmount.Value);
				}
				else
				{
					OrderInfo orderInfo = orderInfos.FirstOrDefault((OrderInfo d) => d.Id == productTotalAmount.OrderId);
					if (orderInfo == null)
					{
						continue;
					}
					productTotalAmount.EnabledRefundAmount = (orderInfo.ProductTotalAmount + orderInfo.Freight) - orderInfo.DiscountAmount;
				}
			}
			PageModel<OrderRefundInfo> pageModel = new PageModel<OrderRefundInfo>()
			{
				Models = list.AsQueryable<OrderRefundInfo>(),
				Total = num
			};
			return pageModel;
		}

		public void SellerConfirmRefundGood(long id, string sellerName)
		{
			OrderRefundInfo nullable = context.OrderRefundInfo.FindById<OrderRefundInfo>(id);
			if (nullable.SellerAuditStatus != OrderRefundInfo.OrderRefundAuditStatus.WaitReceiving)
			{
				throw new HimallException("只有待收货状态的退货才能进行确认收货操作");
			}
			nullable.SellerAuditStatus = OrderRefundInfo.OrderRefundAuditStatus.Audited;
			nullable.SellerConfirmArrivalDate = new DateTime?(DateTime.Now);
			nullable.ManagerConfirmDate = DateTime.Now;
			OrderOperationLogInfo orderOperationLogInfo = new OrderOperationLogInfo()
			{
				Operator = sellerName,
				OrderId = nullable.OrderId,
				OperateDate = DateTime.Now,
				OperateContent = "商家确认收到退货"
			};
            context.OrderOperationLogInfo.Add(orderOperationLogInfo);
            context.SaveChanges();
		}

		public void SellerDealRefund(long id, OrderRefundInfo.OrderRefundAuditStatus auditStatus, string sellerRemark, string sellerName)
		{
			OrderRefundInfo now = context.OrderRefundInfo.FindById<OrderRefundInfo>(id);
			if (now.SellerAuditStatus != OrderRefundInfo.OrderRefundAuditStatus.WaitAudit)
			{
				throw new HimallException("只有待审核状态的退款/退货才能进行处理");
			}
			if (now.RefundMode == OrderRefundInfo.OrderRefundMode.OrderRefund)
			{
				if (auditStatus == OrderRefundInfo.OrderRefundAuditStatus.WaitDelivery)
				{
					auditStatus = OrderRefundInfo.OrderRefundAuditStatus.Audited;
					Instance<IOrderService>.Create.AgreeToRefundBySeller(now.OrderId);
				}
			}
			else if (!now.IsReturn && auditStatus == OrderRefundInfo.OrderRefundAuditStatus.WaitDelivery)
			{
				auditStatus = OrderRefundInfo.OrderRefundAuditStatus.Audited;
			}
			if (auditStatus == OrderRefundInfo.OrderRefundAuditStatus.UnAudit)
			{
				if (now.RefundMode != OrderRefundInfo.OrderRefundMode.OrderRefund)
				{
					OrderItemInfo num = context.OrderItemInfo.FindById<OrderItemInfo>(now.OrderItemId);
					if (num != null)
					{
						num.ReturnQuantity = 0;
						num.RefundPrice = new decimal(0);
					}
				}
				else
				{
					OrderInfo orderInfo = context.OrderInfo.Include("OrderItemInfo").FirstOrDefault((OrderInfo d) => d.Id == now.OrderId);
					if (orderInfo != null)
					{
						foreach (OrderItemInfo orderItemInfo in orderInfo.OrderItemInfo)
						{
							orderItemInfo.ReturnQuantity = 0;
							orderItemInfo.RefundPrice = new decimal(0);
						}
					}
				}
			}
			if (auditStatus != OrderRefundInfo.OrderRefundAuditStatus.WaitDelivery || now.IsReturn)
			{
				now.SellerAuditStatus = auditStatus;
			}
			else
			{
				now.SellerAuditStatus = OrderRefundInfo.OrderRefundAuditStatus.Audited;
			}
			now.SellerAuditDate = DateTime.Now;
			now.SellerRemark = sellerRemark;
			if (auditStatus == OrderRefundInfo.OrderRefundAuditStatus.Audited)
			{
				now.ManagerConfirmDate = DateTime.Now;
			}
			OrderOperationLogInfo orderOperationLogInfo = new OrderOperationLogInfo()
			{
				Operator = sellerName,
				OrderId = now.OrderId,
				OperateDate = DateTime.Now,
				OperateContent = "商家处理退款退货申请"
			};
            context.OrderOperationLogInfo.Add(orderOperationLogInfo);
            context.SaveChanges();
		}

		private void UpdateProductVisti(OrderRefundInfo refund, DateTime payDate)
		{
			OrderItemInfo orderItemInfo = refund.OrderItemInfo;
			ProductInfo productInfo = new ProductInfo();
			ProductVistiInfo productVistiInfo = new ProductVistiInfo();
			if (refund.RefundMode == OrderRefundInfo.OrderRefundMode.OrderRefund)
			{
				List<OrderItemInfo> list = (
					from d in context.OrderItemInfo
					where d.OrderId == refund.OrderId
					select d).ToList();
				foreach (OrderItemInfo orderItemInfo1 in list)
				{
					productInfo = context.ProductInfo.FirstOrDefault((ProductInfo d) => d.Id == orderItemInfo1.ProductId);
					if (productInfo != null)
					{
						ProductInfo saleCounts = productInfo;
						saleCounts.SaleCounts = saleCounts.SaleCounts - orderItemInfo1.Quantity;
					}
					productVistiInfo = context.ProductVistiInfo.FindBy((ProductVistiInfo d) => d.ProductId == orderItemInfo1.ProductId && (d.Date == payDate.Date)).FirstOrDefault();
					if (productVistiInfo == null)
					{
						continue;
					}
					ProductVistiInfo saleCounts1 = productVistiInfo;
					saleCounts1.SaleCounts = saleCounts1.SaleCounts - orderItemInfo.Quantity;
					ProductVistiInfo saleAmounts = productVistiInfo;
					saleAmounts.SaleAmounts = saleAmounts.SaleAmounts - refund.Amount;
				}
			}
			else if (refund.IsReturn)
			{
				productInfo = context.ProductInfo.FirstOrDefault((ProductInfo d) => d.Id == refund.OrderItemInfo.ProductId);
				if (productInfo != null)
				{
					ProductInfo productInfo1 = productInfo;
					productInfo1.SaleCounts = productInfo1.SaleCounts - refund.OrderItemInfo.ReturnQuantity;
				}
				productVistiInfo = context.ProductVistiInfo.FindBy((ProductVistiInfo item) => item.ProductId == orderItemInfo.ProductId && (item.Date == payDate.Date)).FirstOrDefault();
				if (productVistiInfo != null)
				{
					ProductVistiInfo productVistiInfo1 = productVistiInfo;
					productVistiInfo1.SaleCounts = productVistiInfo1.SaleCounts - orderItemInfo.Quantity;
					ProductVistiInfo saleAmounts1 = productVistiInfo;
					saleAmounts1.SaleAmounts = saleAmounts1.SaleAmounts - refund.Amount;
				}
			}
            context.SaveChanges();
		}

		private void UpdateShopVisti(OrderRefundInfo refund, DateTime payDate)
		{
			ShopVistiInfo shopVistiInfo = context.ShopVistiInfo.FindBy((ShopVistiInfo item) => item.ShopId == refund.ShopId && (item.Date == payDate.Date)).FirstOrDefault();
			if (shopVistiInfo != null)
			{
				if (refund.RefundMode == OrderRefundInfo.OrderRefundMode.OrderRefund)
				{
					List<OrderItemInfo> list = (
						from d in context.OrderItemInfo
						where d.OrderId == refund.OrderId
						select d).ToList();
					foreach (OrderItemInfo orderItemInfo in list)
					{
						ShopVistiInfo saleCounts = shopVistiInfo;
						saleCounts.SaleCounts = saleCounts.SaleCounts - orderItemInfo.Quantity;
					}
				}
				else if (refund.IsReturn)
				{
					ShopVistiInfo saleCounts1 = shopVistiInfo;
					saleCounts1.SaleCounts = saleCounts1.SaleCounts - refund.OrderItemInfo.ReturnQuantity;
				}
				ShopVistiInfo saleAmounts = shopVistiInfo;
				saleAmounts.SaleAmounts = saleAmounts.SaleAmounts - refund.Amount;
                context.SaveChanges();
			}
		}

		public void UserConfirmRefundGood(long id, string sellerName, string expressCompanyName, string shipOrderNumber)
		{
			OrderRefundInfo nullable = context.OrderRefundInfo.FindById<OrderRefundInfo>(id);
			if (nullable.SellerAuditStatus != OrderRefundInfo.OrderRefundAuditStatus.WaitDelivery)
			{
				throw new HimallException("只有待等待发货状态的能进行发货操作");
			}
			nullable.ShipOrderNumber = shipOrderNumber;
			nullable.ExpressCompanyName = expressCompanyName;
			nullable.SellerAuditStatus = OrderRefundInfo.OrderRefundAuditStatus.WaitReceiving;
			nullable.BuyerDeliverDate = new DateTime?(DateTime.Now);
			OrderOperationLogInfo orderOperationLogInfo = new OrderOperationLogInfo()
			{
				Operator = sellerName,
				OrderId = nullable.OrderId,
				OperateDate = DateTime.Now,
				OperateContent = "买家确认发回商品"
			};
            context.OrderOperationLogInfo.Add(orderOperationLogInfo);
            context.SaveChanges();
		}
	}
}