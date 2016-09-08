using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class ProductConcernController : BaseMemberController
	{
		public ProductConcernController()
		{
		}

		public JsonResult CancelConcernProducts(string ids)
		{
			string[] strArrays = ids.Split(new char[] { ',' });
			List<long> nums = new List<long>();
			string[] strArrays1 = strArrays;
			for (int i = 0; i < strArrays1.Length; i++)
			{
				nums.Add(Convert.ToInt64(strArrays1[i]));
			}
			ServiceHelper.Create<IProductService>().CancelConcernProducts(nums, base.CurrentUser.Id);
			Result result = new Result()
			{
				success = true,
				msg = "Cancel Success！"
			};
			return Json(result);
		}

		public ActionResult Index(int pageSize = 10, int pageNo = 1)
		{
			PageModel<FavoriteInfo> userConcernProducts = ServiceHelper.Create<IProductService>().GetUserConcernProducts(base.CurrentUser.Id, pageNo, pageSize);
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = userConcernProducts.Total
			};
			ViewBag.pageInfo = pagingInfo;
			return View(userConcernProducts.Models.ToList());
		}
	}
}