using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class VShopController : BaseAdminController
	{
		public VShopController()
		{
		}

		public JsonResult DeleteHotVShop(int vshopId)
		{
			ServiceHelper.Create<IVShopService>().DeleteHotShop(vshopId);
			return Json(new { success = true });
		}

		public JsonResult DeleteTopShow(long vshopId)
		{
			ServiceHelper.Create<IVShopService>().DeleteTopShop(vshopId);
			return Json(new { success = true });
		}

		public JsonResult DeleteVshop(long vshopId)
		{
			ServiceHelper.Create<IVShopService>().CloseShop(vshopId);
			return Json(new { success = true });
		}

		public JsonResult GetHotShop(int page, int rows, string vshopName, DateTime? startTime = null, DateTime? endTime = null)
		{
			int num;
			VshopQuery vshopQuery = new VshopQuery()
			{
				PageNo = page,
				PageSize = rows,
				Name = vshopName
			};
			VshopQuery vshopQuery1 = vshopQuery;
			List<VShopInfo> list = ServiceHelper.Create<IVShopService>().GetHotShop(vshopQuery1, startTime, endTime, out num).ToList();
			num = list.Count();
			var variable = 
				from item in list
				select new { id = item.Id, name = item.Name, squence = item.VShopExtendInfo.FirstOrDefault().Sequence, addTime = item.VShopExtendInfo.FirstOrDefault().AddTime.ToString(), creatTime = item.CreateTime.ToString(), visiteNum = item.VisitNum, buyNum = item.buyNum };
			return Json(new { rows = variable, total = num });
		}

		public JsonResult GetVshops(int page, int rows, string vshopName, int? vshopType = null)
		{
			Func<VShopExtendInfo, bool> func = null;
			Func<VShopExtendInfo, bool> func1 = null;
			int num = 0;
			VshopQuery vshopQuery = new VshopQuery()
			{
				Name = vshopName,
				PageNo = page,
				PageSize = rows
			};
			VshopQuery nullable = vshopQuery;
			int? nullable1 = vshopType;
			if ((nullable1.GetValueOrDefault() != 1 ? false : nullable1.HasValue))
			{
				nullable.VshopType = new VShopExtendInfo.VShopExtendType?(VShopExtendInfo.VShopExtendType.TopShow);
			}
			int? nullable2 = vshopType;
			if ((nullable2.GetValueOrDefault() != 2 ? false : nullable2.HasValue))
			{
				nullable.VshopType = new VShopExtendInfo.VShopExtendType?(VShopExtendInfo.VShopExtendType.HotVShop);
			}
			int? nullable3 = vshopType;
			if ((nullable3.GetValueOrDefault() != 0 ? false : nullable3.HasValue))
			{
				nullable.VshopType = new VShopExtendInfo.VShopExtendType?(0);
			}
			List<VShopInfo> list = ServiceHelper.Create<IVShopService>().GetVShopByParamete(nullable, out num).ToList();
			num = list.Count();
			ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
			IShopCategoryService shopCategoryService = ServiceHelper.Create<IShopCategoryService>();
			var collection = list.ToArray().Select((VShopInfo item) => {
				string str;
				long id = item.Id;
				string name = item.Name;
				string str1 = item.CreateTime.ToString();
				ICollection<VShopExtendInfo> vShopExtendInfo = item.VShopExtendInfo;
				if (func == null)
				{
					func = (VShopExtendInfo t) => t.Type == VShopExtendInfo.VShopExtendType.TopShow;
				}
				if (vShopExtendInfo.Any(func))
				{
					str = "主推微店";
				}
				else
				{
					ICollection<VShopExtendInfo> vShopExtendInfos = item.VShopExtendInfo;
					if (func1 == null)
					{
						func1 = (VShopExtendInfo t) => t.Type == VShopExtendInfo.VShopExtendType.HotVShop;
					}
					str = (vShopExtendInfos.Any(func1) ? "热门微店" : "普通微店");
				}
				return new { id = id, name = name, creatTime = str1, vshopTypes = str, categoryName = (shopCategoryService.GetBusinessCategory(item.ShopId).FirstOrDefault() != null ? categoryService.GetCategory(long.Parse(shopCategoryService.GetBusinessCategory(item.ShopId).FirstOrDefault().Path.Split(new char[] { '|' }).First<string>())).Name : ""), visiteNum = item.VisitNum, buyNum = item.buyNum };
			});
			return Json(new { rows = collection, total = num });
		}

		public ActionResult HotVShop()
		{
			return View();
		}

		public ActionResult Index()
		{
			return View();
		}

		public JsonResult ReplaceHotShop(long oldVShopId, long newHotVShopId)
		{
			ServiceHelper.Create<IVShopService>().ReplaceHotShop(oldVShopId, newHotVShopId);
			return Json(new { success = true });
		}

		public JsonResult ReplaceTopShop(long oldVShopId, long newVShopId)
		{
			ServiceHelper.Create<IVShopService>().ReplaceTopShop(oldVShopId, newVShopId);
			return Json(new { success = true });
		}

		public JsonResult SetHotVshop(long vshopId)
		{
			ServiceHelper.Create<IVShopService>().SetHotShop(vshopId);
			return Json(new { success = true });
		}

		public JsonResult SetTopVshop(long vshopId)
		{
			ServiceHelper.Create<IVShopService>().SetTopShop(vshopId);
			return Json(new { success = true });
		}

		public void testout(out int t)
		{
			t = 15;
		}

		public ActionResult TopShop()
		{
			return View(ServiceHelper.Create<IVShopService>().GetTopShop());
		}

		public JsonResult UpdateSequence(long id, int? sequence)
		{
			ServiceHelper.Create<IVShopService>().UpdateSequence(id, sequence);
			return Json(new { success = true });
		}

		public ActionResult VShopManagement()
		{
			return View();
		}
	}
}