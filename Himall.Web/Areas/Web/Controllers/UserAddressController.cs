using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class UserAddressController : BaseMemberController
	{
		public UserAddressController()
		{
		}

		[HttpPost]
		public JsonResult AddShippingAddress(ShippingAddressInfo info)
		{
			info.UserId = base.CurrentUser.Id;
			ServiceHelper.Create<IShippingAddressService>().AddShippingAddress(info);
			return Json(new { success = true, msg = "添加成功", id = info.Id });
		}

		[HttpPost]
		public JsonResult DeleteShippingAddress(long id)
		{
			long num = base.CurrentUser.Id;
			ServiceHelper.Create<IShippingAddressService>().DeleteShippingAddress(id, num);
			Result result = new Result()
			{
				success = true,
				msg = "删除成功"
			};
			return Json(result);
		}

		[HttpPost]
		public JsonResult EditShippingAddress(ShippingAddressInfo info)
		{
			info.UserId = base.CurrentUser.Id;
			ServiceHelper.Create<IShippingAddressService>().UpdateShippingAddress(info);
			return Json(new { success = true, msg = "修改成功", id = info.Id });
		}

		[HttpPost]
		public JsonResult GetShippingAddress(long id)
		{
			ShippingAddressInfo userShippingAddress = ServiceHelper.Create<IShippingAddressService>().GetUserShippingAddress(id);
			var variable = new { id = userShippingAddress.Id, fullRegionName = userShippingAddress.RegionFullName, address = userShippingAddress.Address, phone = userShippingAddress.Phone, shipTo = userShippingAddress.ShipTo, fullRegionIdPath = userShippingAddress.RegionIdPath };
			return Json(variable);
		}

		public ActionResult Index()
		{
			long id = base.CurrentUser.Id;
			return View(ServiceHelper.Create<IShippingAddressService>().GetUserShippingAddressByUserId(id));
		}

		[HttpPost]
		public JsonResult SetDefaultShippingAddress(long id)
		{
			long num = base.CurrentUser.Id;
			ServiceHelper.Create<IShippingAddressService>().SetDefaultShippingAddress(id, num);
			Result result = new Result()
			{
				success = true,
				msg = "设置成功"
			};
			return Json(result);
		}

		[HttpPost]
		public JsonResult SetQuickShippingAddress(long id)
		{
			long num = base.CurrentUser.Id;
			ServiceHelper.Create<IShippingAddressService>().SetQuickShippingAddress(id, num);
			Result result = new Result()
			{
				success = true,
				msg = "设置成功"
			};
			return Json(result);
		}
	}
}