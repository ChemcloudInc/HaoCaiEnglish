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
	public class ComplaintService : ServiceBase, IComplaintService, IService, IDisposable
	{
		public ComplaintService()
		{
		}

		public void AddComplaint(OrderComplaintInfo model)
		{
			if (context.OrderComplaintInfo.Any((OrderComplaintInfo a) => a.OrderId == model.OrderId))
			{
				throw new HimallException("你已经投诉过了，请勿重复投诉！");
			}
			if (string.IsNullOrEmpty(model.SellerReply))
			{
				model.SellerReply = string.Empty;
			}
            context.OrderComplaintInfo.Add(model);
            context.SaveChanges();
		}

		public void DealComplaint(long id)
		{
			OrderComplaintInfo orderComplaintInfo = context.OrderComplaintInfo.FindById<OrderComplaintInfo>(id);
			orderComplaintInfo.Status = OrderComplaintInfo.ComplaintStatus.End;
            context.SaveChanges();
		}

		public IQueryable<OrderComplaintInfo> GetAllComplaint()
		{
			return context.OrderComplaintInfo.FindAll<OrderComplaintInfo>();
		}

		public PageModel<OrderComplaintInfo> GetOrderComplaints(ComplaintQuery complaintQuery)
		{
			int num;
			List<long> nums = new List<long>();
			IQueryable<OrderComplaintInfo> orderId = context.OrderComplaintInfo.AsQueryable<OrderComplaintInfo>();
			if (complaintQuery.OrderId.HasValue)
			{
				orderId = 
					from item in orderId
					where complaintQuery.OrderId == item.OrderId
                    select item;
			}
			if (complaintQuery.StartDate.HasValue)
			{
				orderId = 
					from item in orderId
					where complaintQuery.StartDate <= item.ComplaintDate
                    select item;
			}
			if (complaintQuery.EndDate.HasValue)
			{
				orderId = 
					from item in orderId
					where complaintQuery.EndDate >= item.ComplaintDate
                    select item;
			}
			if (complaintQuery.Status.HasValue)
			{
				orderId = 
					from item in orderId
					where (int?)complaintQuery.Status == (int?)item.Status
					select item;
			}
			if (complaintQuery.ShopId.HasValue)
			{
				orderId = 
					from item in orderId
					where complaintQuery.ShopId == item.ShopId
                    select item;
			}
			if (complaintQuery.UserId.HasValue)
			{
				orderId = 
					from item in orderId
					where item.UserId == complaintQuery.UserId
                    select item;
			}
			if (!string.IsNullOrWhiteSpace(complaintQuery.ShopName))
			{
				orderId = 
					from item in orderId
					where item.ShopName.Contains(complaintQuery.ShopName)
					select item;
			}
			if (!string.IsNullOrWhiteSpace(complaintQuery.UserName))
			{
				orderId = 
					from item in orderId
					where item.UserName.Contains(complaintQuery.UserName)
					select item;
			}
			orderId = orderId.GetPage(out num, complaintQuery.PageNo, complaintQuery.PageSize, null);
			return new PageModel<OrderComplaintInfo>()
			{
				Models = orderId,
				Total = num
			};
		}

		public void SellerDealComplaint(long id, string reply)
		{
			OrderComplaintInfo orderComplaintInfo = context.OrderComplaintInfo.FindById<OrderComplaintInfo>(id);
			orderComplaintInfo.Status = OrderComplaintInfo.ComplaintStatus.Dealed;
			orderComplaintInfo.SellerReply = reply;
            context.SaveChanges();
		}

		public void UserApplyArbitration(long id, long userId)
		{
			OrderComplaintInfo orderComplaintInfo = context.OrderComplaintInfo.FindById<OrderComplaintInfo>(id);
			if (orderComplaintInfo.UserId != userId)
			{
				throw new HimallException("该投诉不属于此用户！");
			}
			orderComplaintInfo.Status = OrderComplaintInfo.ComplaintStatus.Dispute;
            context.SaveChanges();
		}

		public void UserDealComplaint(long id, long userId)
		{
			OrderComplaintInfo orderComplaintInfo = context.OrderComplaintInfo.FindById<OrderComplaintInfo>(id);
			if (orderComplaintInfo.UserId != userId)
			{
				throw new HimallException("该投诉不属于此用户！");
			}
			orderComplaintInfo.Status = OrderComplaintInfo.ComplaintStatus.End;
            context.SaveChanges();
		}
	}
}