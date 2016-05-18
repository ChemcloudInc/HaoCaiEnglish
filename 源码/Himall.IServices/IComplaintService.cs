using Himall.IServices.QueryModel;
using Himall.Model;
using System;

namespace Himall.IServices
{
	public interface IComplaintService : IService, IDisposable
	{
		void AddComplaint(OrderComplaintInfo info);

		void DealComplaint(long id);

		PageModel<OrderComplaintInfo> GetOrderComplaints(ComplaintQuery complaintQuery);

		void SellerDealComplaint(long id, string reply);

		void UserApplyArbitration(long id, long userId);

		void UserDealComplaint(long id, long userId);
	}
}