using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web.Areas.Web.Models;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Helper
{
	public class ShopServiceMark
	{
		public ShopServiceMark()
		{
		}

		public static ShopServiceMarkModel GetShopComprehensiveMark(long shopId)
		{
			ShopServiceMarkModel shopServiceMarkModel = new ShopServiceMarkModel();
			ITradeCommentService create = Instance<ITradeCommentService>.Create;
			OrderCommentQuery orderCommentQuery = new OrderCommentQuery()
			{
				ShopId = new long?(shopId),
				PageNo = 1,
				PageSize = 100000
			};
			PageModel<OrderCommentInfo> orderComments = create.GetOrderComments(orderCommentQuery);
			shopServiceMarkModel.PackMark = (orderComments.Models.Count() == 0 ? new decimal(0) : Math.Round(orderComments.Models.ToList().Average((OrderCommentInfo o) => (o.PackMark + o.DeliveryMark) / new decimal(2)), 2));
			shopServiceMarkModel.ServiceMark = (orderComments.Models.Count() == 0 ? new decimal(0) : Math.Round((decimal)orderComments.Models.ToList().Average((OrderCommentInfo o) => o.ServiceMark), 2));
			shopServiceMarkModel.ComprehensiveMark = Math.Round((shopServiceMarkModel.PackMark + shopServiceMarkModel.ServiceMark) / new decimal(2), 2);
			return shopServiceMarkModel;
		}
	}
}