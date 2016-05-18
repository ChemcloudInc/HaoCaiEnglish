using Himall.IServices;
using Himall.Model;
using Himall.Model.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.AdvancedAPIs.Poi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class PoiController : BaseSellerController
	{
		private IPoiService _poiService;

		private WXShopInfo _settings;

		private bool _isdeploy = true;

		public PoiController()
		{
            _settings = ServiceHelper.Create<IVShopService>().GetVShopSetting(base.CurrentSellerManager.ShopId);
			if (_settings != null)
			{
                _poiService = ServiceHelper.Create<IPoiService>();
				try
				{
                    _poiService.init(_settings.AppId, _settings.AppSecret);
				}
				catch
				{
                    _isdeploy = false;
				}
			}
		}

		public ActionResult AddPage()
		{
			ViewBag.Category = _poiService.GetCategory();
			return View();
		}

		public ActionResult AddPoi(StoreBaseInfo poidata)
		{
			CreateStoreData createStoreDatum = new CreateStoreData()
			{
				business = new CreateStore_Business()
				{
					base_info = poidata
				}
			};
			bool flag = _poiService.AddPoi(createStoreDatum);
			return Json(new { success = flag });
		}

		[HttpPost]
		public ActionResult DelPoi(string id)
		{
			bool flag = _poiService.DeletePoi(id);
			return Json(new { success = flag });
		}

		public ActionResult Detail(string id)
		{
			GetStoreBaseInfo poi = _poiService.GetPoi(id);
			ViewBag.Id = id;
			return View(poi);
		}

		public ActionResult EditPage(string id)
		{
			GetStoreBaseInfo poi = _poiService.GetPoi(id);
			ViewBag.Id = id;
			ViewBag.Category = _poiService.GetCategory();
			return View(poi);
		}

		public ActionResult Index()
		{
			if (_settings == null)
			{
				return Redirect("/sellerAdmin/Poi/UnConfig/您还未绑定微信公众号，需要绑定微信已认证服务号才可以使用摇一摇及门店管理相关功能。");
			}
			if (!_isdeploy)
			{
				return Redirect("/sellerAdmin/Poi/UnConfig/access_token错误或失效，请确认AppId与AppSecret配置正确");
			}
			if (_poiService.GetPoiList(0, 1).errcode != ReturnCode.api功能未授权)
			{
				return View();
			}
			return Redirect("/sellerAdmin/Poi/Unauthorized");
		}

		[HttpPost]
		public ActionResult List(int page, int rows)
		{
			GetStoreListResultJson poiList = _poiService.GetPoiList(page, rows);
			List<GetStoreList_BaseInfo> list = (
				from l in poiList.business_list
				select l.base_info).ToList();
			DataGridModel<GetStoreList_BaseInfo> dataGridModel = new DataGridModel<GetStoreList_BaseInfo>()
			{
				rows = list,
				total = int.Parse(poiList.total_count)
			};
			return Json(dataGridModel);
		}

		[HttpPost]
		public ActionResult Save(PoiEditModel model)
		{
			string str = "";
			if (!string.IsNullOrEmpty(model.photo_list) && model.photo_list.IndexOf("http") < 0)
			{
				string str1 = Server.MapPath(string.Concat("~", model.photo_list));
				str = _poiService.UploadImage(str1);
			}
			if (model.id != 0)
			{
				UpdateStore_BaseInfo updateStoreBaseInfo = new UpdateStore_BaseInfo()
				{
					poi_id = model.poi_id,
					telephone = model.telephone,
					recommend = model.recommend,
					special = model.special,
					introduction = model.introduction,
					open_time = model.open_time,
					avg_price = model.avg_price
				};
				List<Store_Photo> storePhotos = new List<Store_Photo>()
				{
					new Store_Photo()
					{
						photo_url = str
					}
				};
				updateStoreBaseInfo.photo_list = storePhotos;
				UpdateStoreData updateStoreDatum = new UpdateStoreData()
				{
					business = new UpdateStore_Business()
					{
						base_info = updateStoreBaseInfo
					}
				};
				bool flag = _poiService.UpdatePoi(updateStoreDatum);
				return Json(new { success = flag });
			}
			StoreBaseInfo storeBaseInfo = new StoreBaseInfo()
			{
				sid = Guid.NewGuid().ToString(),
				province = model.province,
				city = model.city,
				district = model.district,
				address = model.address,
				business_name = model.business_name,
				branch_name = model.branch_name
			};
			string[] strArrays = new string[] { string.Concat(model.categoryOne, ",", model.categoryTwo) };
			storeBaseInfo.categories = strArrays;
			List<Store_Photo> storePhotos1 = new List<Store_Photo>()
			{
				new Store_Photo()
				{
					photo_url = str
				}
			};
			storeBaseInfo.photo_list = storePhotos1;
			storeBaseInfo.telephone = model.telephone;
			storeBaseInfo.avg_price = model.avg_price;
			storeBaseInfo.open_time = model.open_time;
			storeBaseInfo.offset_type = 1;
			storeBaseInfo.recommend = model.recommend;
			storeBaseInfo.special = model.special;
			storeBaseInfo.longitude = "0";
			storeBaseInfo.latitude = "0";
			storeBaseInfo.introduction = model.introduction;
			CreateStoreData createStoreDatum = new CreateStoreData()
			{
				business = new CreateStore_Business()
				{
					base_info = storeBaseInfo
				}
			};
			bool flag1 = _poiService.AddPoi(createStoreDatum);
			return Json(new { success = flag1 });
		}

		public ActionResult Unauthorized()
		{
			return View();
		}

		public ActionResult UnConfig(string id)
		{
			ViewBag.Name = id;
			return View();
		}
	}
}