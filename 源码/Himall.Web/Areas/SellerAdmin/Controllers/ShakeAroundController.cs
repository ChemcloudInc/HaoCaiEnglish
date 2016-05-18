using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.Poi;
using Senparc.Weixin.MP.AdvancedAPIs.ShakeAround;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class ShakeAroundController : BaseSellerController
	{
		private IShakeAroundService _saService;

		private IPoiService _poiService;

		private WXShopInfo _settings;

		private bool _isdeploy = true;

		public ShakeAroundController()
		{
            _settings = ServiceHelper.Create<IVShopService>().GetVShopSetting(base.CurrentSellerManager.ShopId);
            _saService = ServiceHelper.Create<IShakeAroundService>();
            _poiService = ServiceHelper.Create<IPoiService>();
			try
			{
                _saService.init(_settings.AppId, _settings.AppSecret);
                _poiService.init(_settings.AppId, _settings.AppSecret);
			}
			catch
			{
                _isdeploy = false;
			}
		}

		public ActionResult AddPage()
		{
			return View();
		}

		public ActionResult AddSa()
		{
			List<GetStoreList_BaseInfo> poiList = _poiService.GetPoiList();
			base.ViewBag.Poi = (
				from p in poiList
				where p.available_state == 3
				select p).ToList();
			return View();
		}

		[HttpPost]
		public ActionResult BindRelationship(BindRelationshipModel model)
		{
			DeviceModel deviceById = _saService.GetDeviceById(model.id);
			DeviceApply_Data_Device_Identifiers deviceApplyDataDeviceIdentifier = new DeviceApply_Data_Device_Identifiers()
			{
				device_id = long.Parse(deviceById.device_id),
				major = long.Parse(deviceById.major),
				uuid = deviceById.uuid,
				minor = long.Parse(deviceById.minor)
			};
			IShakeAroundService shakeAroundService = _saService;
			DeviceApply_Data_Device_Identifiers deviceApplyDataDeviceIdentifier1 = deviceApplyDataDeviceIdentifier;
			string str = model.pageids;
			char[] chrArray = new char[] { ',' };
			bool flag = shakeAroundService.SetRelationship(deviceApplyDataDeviceIdentifier1, (
				from p in str.Split(chrArray)
				select long.Parse(p)).ToArray(), ShakeAroundBindType.建立关联关系);
			return Json(new { success = flag });
		}

		[HttpPost]
		public ActionResult DeletePage(long id)
		{
			IShakeAroundService shakeAroundService = _saService;
			List<long> nums = new List<long>()
			{
				id
			};
			return Json(new { success = shakeAroundService.DeletePage(nums) });
		}

		[HttpPost]
		public ActionResult DeviceBindLocatoin(DeviceEditModel model)
		{
			bool flag = _saService.DeviceBindLocatoin(model.device_id, model.uuid, model.major, model.minor, model.poi_id);
			return Json(new { success = flag });
		}

		public ActionResult EditPage(long id)
		{
			long[] numArray = new long[] { id };
			SearchPages_Data_Page item = _saService.GetPageById(numArray)[0];
			return View(item);
		}

		[HttpPost]
		public ActionResult GetPagesByNotRelationship(long id, int page, int rows)
		{
			SearchPages_Data pageAll = _saService.GetPageAll();
			DeviceModel deviceById = _saService.GetDeviceById(id);
			List<long> pageids = _saService.GetPageids(deviceById);
			string[] array = new string[0];
			if (pageids != null && pageids.Count > 0)
			{
				array = (
					from p in pageids
					select p.ToString()).ToArray();
			}
			List<SearchPages_Data_Page> list = (
				from p in pageAll.pages
				where !array.Contains<string>(p.page_id.ToString())
				select p).ToList();
			DataGridModel<SearchPages_Data_Page> dataGridModel = new DataGridModel<SearchPages_Data_Page>()
			{
				rows = list,
				total = list.Count
			};
			return Json(dataGridModel);
		}

		[HttpPost]
		public ActionResult GetPoiList()
		{
			List<GetStoreList_BaseInfo> list = (
				from p in _poiService.GetPoiList()
				where p.available_state == 3
				select p).ToList();
			return Json(list);
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
			if (_saService.UnauthorizedTest().errcode != ReturnCode.api功能未授权)
			{
				return View();
			}
			return Redirect("/sellerAdmin/Poi/Unauthorized");
		}

		[HttpPost]
		public ActionResult List(int page, int rows)
		{
			List<DeviceShowModel> deviceShowModels = new List<DeviceShowModel>();
			DeviceList2ResultJson deviceAll = _saService.GetDeviceAll(page, rows);
			List<DeviceModel> deviceModels = deviceAll.devices;
			List<GetStoreList_BaseInfo> poiList = _poiService.GetPoiList();
			foreach (DeviceModel deviceModel in deviceModels)
			{
				string str = (
					from p in poiList
					where p.poi_id == deviceModel.poi_id
					select p.business_name).FirstOrDefault();
				deviceShowModels.Add(new DeviceShowModel(deviceModel)
				{
					poi_name = str
				});
			}
			DataGridModel<DeviceShowModel> dataGridModel = new DataGridModel<DeviceShowModel>()
			{
				rows = deviceShowModels,
				total = deviceAll.total_count
			};
			return Json(dataGridModel);
		}

		public ActionResult PageIndex()
		{
			if (_settings == null)
			{
				return Redirect("/sellerAdmin/Poi/UnConfig/您还未绑定微信公众号，需要绑定微信已认证服务号才可以使用摇一摇及门店管理相关功能。");
			}
			if (!_isdeploy)
			{
				return Redirect("/sellerAdmin/Poi/UnConfig/access_token错误或失效，请确认AppId与AppSecret配置正确");
			}
			if (_saService.UnauthorizedTest().errcode != ReturnCode.api功能未授权)
			{
				return View();
			}
			return Redirect("/sellerAdmin/Poi/Unauthorized");
		}

		[HttpPost]
		public ActionResult PageList(int page, int rows)
		{
			SearchPages_Data pageAll = _saService.GetPageAll(page, rows);
			DataGridModel<SearchPages_Data_Page> dataGridModel = new DataGridModel<SearchPages_Data_Page>()
			{
				rows = pageAll.pages,
				total = pageAll.total_count
			};
			return Json(dataGridModel);
		}

		public ActionResult Relationship(long id)
		{
			DeviceModel deviceById = _saService.GetDeviceById(id);
			List<long> pageids = _saService.GetPageids(deviceById);
			List<SearchPages_Data_Page> searchPagesDataPages = new List<SearchPages_Data_Page>();
			if (pageids != null && pageids.Count > 0)
			{
				searchPagesDataPages = _saService.GetPageById(pageids.ToArray());
			}
			ViewBag.DevModel = deviceById;
			ViewBag.PageModel = searchPagesDataPages;
			return View();
		}

		[HttpPost]
		public ActionResult RemoveRelationship(RemoveRelationshipModel model)
		{
			DeviceModel deviceById = _saService.GetDeviceById(model.id);
			DeviceApply_Data_Device_Identifiers deviceApplyDataDeviceIdentifier = new DeviceApply_Data_Device_Identifiers()
			{
				device_id = long.Parse(deviceById.device_id),
				major = long.Parse(deviceById.major),
				uuid = deviceById.uuid,
				minor = long.Parse(deviceById.minor)
			};
			IShakeAroundService shakeAroundService = _saService;
			long[] numArray = new long[] { model.pageid };
			bool flag = shakeAroundService.SetRelationship(deviceApplyDataDeviceIdentifier, numArray, ShakeAroundBindType.解除关联关系);
			return Json(new { success = flag });
		}

		[HttpPost]
		public ActionResult Save(DeviceEditModel model)
		{
			if (model.id == 0)
			{
				bool flag = _saService.AddDevice(model.quantity, model.apply_reason, model.comment, new long?(model.poi_id));
				return Json(new { success = flag });
			}
			bool flag1 = _saService.UpdateDevice(model.device_id, model.uuid, model.major, model.minor, model.comment, model.poi_id);
			return Json(new { success = flag1 });
		}

		[HttpPost]
		public ActionResult SavePage(PageEditModel model)
		{
			if (model.icon_url.IndexOf("http") < 0)
			{
				string str = Server.MapPath(string.Concat("~", model.icon_url));
				using (FileStream fileStream = new FileStream(str, FileMode.Open, FileAccess.Read))
				{
					Image image = Image.FromStream(fileStream);
					if (image.Width != image.Height)
					{
						throw new HimallException("图片必须为正方形");
					}
				}
				model.icon_url = _saService.UploadImage(str);
			}
			if (model.id == 0)
			{
				bool flag = _saService.AddPage(model.title, model.description, model.page_url, model.icon_url, model.comment);
				return Json(new { success = flag });
			}
			bool flag1 = _saService.UpdatePage(model.page_id, model.title, model.description, model.page_url, model.icon_url, model.comment);
			return Json(new { success = flag1 });
		}
	}
}