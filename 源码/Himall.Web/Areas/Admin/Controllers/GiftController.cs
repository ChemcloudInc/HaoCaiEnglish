using AutoMapper;
using Himall.Core;
using Himall.Core.Helper;
using Himall.Core.Plugins;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class GiftController : BaseAdminController
	{
		private IGiftService giftser;

		private IGiftsOrderService orderser;

		public GiftController()
		{
            giftser = ServiceHelper.Create<IGiftService>();
            orderser = ServiceHelper.Create<IGiftsOrderService>();
			Mapper.CreateMap<GiftInfo, GiftViewModel>();
			Mapper.CreateMap<GiftViewModel, GiftInfo>();
		}

		public ActionResult Add()
		{
			GiftViewModel giftViewModel = new GiftViewModel();
			giftViewModel = new GiftViewModel()
			{
				Sequence = 100,
				VirtualSales = 0,
				NeedGrade = 0,
				GiftValue = new decimal?(new decimal(0)),
				EndDate = DateTime.Now.AddMonths(1)
			};
			List<SelectListItem> memberGradeSelectList = GetMemberGradeSelectList(giftViewModel.NeedGrade);
			ViewBag.MemberGradeSelect = memberGradeSelectList;
			return View(giftViewModel);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult ChangeStatus(long id, bool state)
		{
			ServiceHelper.Create<IGiftService>().ChangeStatus(id, state);
			return Json(new { success = true });
		}

		private string ClearHtmlString(string str)
		{
			string str1 = str;
			if (!string.IsNullOrWhiteSpace(str1))
			{
				str1 = str1.Replace("'", "&#39;");
				str1 = str1.Replace("\"", "&#34;");
				str1 = str1.Replace(">", "&gt;");
				str1 = str1.Replace("<", "&lt;");
			}
			return str1;
		}

		public ActionResult Edit(long id)
		{
			GiftViewModel giftViewModel = new GiftViewModel();
			GiftInfo giftInfo = new GiftInfo();
			giftInfo = giftser.GetById(id);
			if (giftInfo == null)
			{
				throw new HimallException("错误的礼品编号。");
			}
			giftViewModel = Mapper.Map<GiftInfo, GiftViewModel>(giftInfo);
			if (string.IsNullOrWhiteSpace(giftViewModel.ImagePath))
			{
				giftViewModel.ImagePath = string.Format("/Storage/Gift/{0}", id);
			}
			string str = Server.MapPath(giftViewModel.ImagePath);
			string[] strArrays = (Directory.Exists(str) ? Directory.GetFiles(str, "?.png") : new string[] { "" });
			if (strArrays.Contains<string>(string.Concat(str, "\\1.png")))
			{
				giftViewModel.PicUrl1 = string.Concat(giftViewModel.ImagePath, "/1.png");
			}
			if (strArrays.Contains<string>(string.Concat(str, "\\2.png")))
			{
				giftViewModel.PicUrl2 = string.Concat(giftViewModel.ImagePath, "/2.png");
			}
			if (strArrays.Contains<string>(string.Concat(str, "\\3.png")))
			{
				giftViewModel.PicUrl3 = string.Concat(giftViewModel.ImagePath, "/3.png");
			}
			if (strArrays.Contains<string>(string.Concat(str, "\\4.png")))
			{
				giftViewModel.PicUrl4 = string.Concat(giftViewModel.ImagePath, "/4.png");
			}
			if (strArrays.Contains<string>(string.Concat(str, "\\5.png")))
			{
				giftViewModel.PicUrl5 = string.Concat(giftViewModel.ImagePath, "/5.png");
			}
			List<SelectListItem> memberGradeSelectList = GetMemberGradeSelectList(giftViewModel.NeedGrade);
			ViewBag.MemberGradeSelect = memberGradeSelectList;
			return View(giftViewModel);
		}

		[HttpPost]
		[OperationLog(Message="操作礼品信息")]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Edit(GiftViewModel model)
		{
			AjaxReturnData ajaxReturnDatum = new AjaxReturnData()
			{
				success = false,
				msg = "未知错误"
			};
			AjaxReturnData ajaxReturnDatum1 = ajaxReturnDatum;
			if (!base.ModelState.IsValid)
			{
				ajaxReturnDatum1.success = false;
				ajaxReturnDatum1.msg = "数据有误";
			}
			else
			{
				GiftViewModel giftViewModel = new GiftViewModel();
				if (model.Id > 0)
				{
					GiftInfo byIdAsNoTracking = giftser.GetByIdAsNoTracking(model.Id);
					if (byIdAsNoTracking == null)
					{
						ajaxReturnDatum1.success = false;
						ajaxReturnDatum1.msg = "编号有误";
						return Json(ajaxReturnDatum1);
					}
					giftViewModel = Mapper.Map<GiftInfo, GiftViewModel>(byIdAsNoTracking);
				}
				else if (model.StockQuantity < 1)
				{
					ajaxReturnDatum1.success = false;
					ajaxReturnDatum1.msg = "库存必须大于0";
					return Json(ajaxReturnDatum1);
				}
				base.UpdateModel<GiftViewModel>(giftViewModel);
				GiftInfo giftInfo = new GiftInfo();
				giftInfo = Mapper.Map<GiftViewModel, GiftInfo>(giftViewModel);
				if (model.Id <= 0)
				{
					giftInfo.Sequence = 100;
					giftInfo.AddDate = new DateTime?(DateTime.Now);
					giftInfo.SalesStatus = GiftInfo.GiftSalesStatus.Normal;
                    giftser.AddGift(giftInfo);
				}
				else
				{
                    giftser.UpdateGift(giftInfo);
				}
				int num = 1;
				List<string> strs = new List<string>()
				{
					model.PicUrl1,
					model.PicUrl2,
					model.PicUrl3,
					model.PicUrl4,
					model.PicUrl5
				};
				foreach (string str in strs)
				{
					string str1 = Server.MapPath(str);
					string str2 = Server.MapPath(giftInfo.ImagePath);
					if (string.IsNullOrWhiteSpace(str))
					{
						string str3 = string.Format("{0}\\{1}.png", str2, num);
						if (System.IO.File.Exists(str3))
						{
                            System.IO.File.Delete(str3);
						}
						IEnumerable<int> dictionary = 
							from t in EnumHelper.ToDictionary<ProductInfo.ImageSize>()
							select t.Key;
						foreach (int num1 in dictionary)
						{
							string str4 = string.Format("{0}/{1}_{2}.png", str2, num, num1);
							if (!System.IO.File.Exists(str4))
							{
								continue;
							}
                            System.IO.File.Delete(str4);
						}
						num++;
					}
					else
					{
						try
						{
							if (!Directory.Exists(str2))
							{
								Directory.CreateDirectory(str2);
							}
							string str5 = string.Format("{0}\\{1}.png", str2, num);
							if (str1 != str5)
							{
								using (Image image = Image.FromFile(str1))
								{
									image.Save(str5, ImageFormat.Png);
									IEnumerable<int> nums = 
										from t in EnumHelper.ToDictionary<GiftInfo.ImageSize>()
										select t.Key;
									foreach (int num2 in nums)
									{
										string str6 = string.Format("{0}/{1}_{2}.png", str2, num, num2);
										ImageHelper.CreateThumbnail(str5, str6, num2, num2);
									}
								}
								num++;
							}
							else
							{
								num++;
							}
						}
						catch (FileNotFoundException fileNotFoundException1)
						{
							FileNotFoundException fileNotFoundException = fileNotFoundException1;
							num++;
							Log.Error("发布礼品时候，没有找到文件", fileNotFoundException);
						}
						catch (ExternalException externalException1)
						{
							ExternalException externalException = externalException1;
							num++;
							Log.Error("发布礼品时候，ExternalException异常", externalException);
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							num++;
							Log.Error("发布礼品时候，Exception异常", exception);
						}
					}
				}
				ajaxReturnDatum1.success = true;
				ajaxReturnDatum1.msg = "操作成功";
			}
			return Json(ajaxReturnDatum1);
		}

		[HttpPost]
		public JsonResult GetExpressData(string expressCompanyName, string shipOrderNumber)
		{
			if (string.IsNullOrWhiteSpace(expressCompanyName) || string.IsNullOrWhiteSpace(shipOrderNumber))
			{
				throw new HimallException("错误的订单信息");
			}
			string kuaidi100Code = ServiceHelper.Create<IExpressService>().GetExpress(expressCompanyName).Kuaidi100Code;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://www.kuaidi100.com/query?type={0}&postid={1}", kuaidi100Code, shipOrderNumber));
			httpWebRequest.Timeout = 8000;
			string end = "暂时没有此快递单号的信息";
			try
			{
				HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK)
				{
					Stream responseStream = response.GetResponseStream();
					StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
					end = streamReader.ReadToEnd();
					end = end.Replace("&amp;", "");
					end = end.Replace("&nbsp;", "");
					end = end.Replace("&", "");
				}
			}
			catch
			{
			}
			return Json(end);
		}

		public List<SelectListItem> GetMemberGradeSelectList(int v)
		{
			List<SelectListItem> list = (
				from d in ServiceHelper.Create<IMemberGradeService>().GetMemberGradeList().ToList()
				select new SelectListItem()
				{
					Text = string.Concat(new object[] { d.GradeName, " (", d.Integral, ")" }),
					Value = d.Id.ToString()
				}).ToList() ?? new List<SelectListItem>();
			SelectListItem selectListItem = new SelectListItem()
			{
				Text = "等级不限",
				Value = "0"
			};
			list.Insert(0, selectListItem);
			SelectListItem selectListItem1 = new SelectListItem()
			{
				Text = "选择会员等级要求",
				Value = ""
			};
			list.Insert(0, selectListItem1);
			foreach (SelectListItem selectListItem2 in list)
			{
				if (selectListItem2.Value != v.ToString())
				{
					continue;
				}
				selectListItem2.Selected = true;
			}
			return list;
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(GiftInfo.GiftSalesStatus? status, string skey, int rows, int page)
		{
			GiftQuery giftQuery = new GiftQuery()
			{
				skey = skey,
				status = status,
				PageSize = rows,
				PageNo = page
			};
			PageModel<GiftModel> gifts = giftser.GetGifts(giftQuery);
			List<GiftModel> list = gifts.Models.ToList();
			return Json(new { rows = list, total = gifts.Total });
		}

		public ActionResult Management()
		{
			return View();
		}

		public ActionResult Order()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult OrderList(string skey, GiftOrderInfo.GiftOrderStatus? status, int rows, int page)
		{
			GiftsOrderQuery giftsOrderQuery = new GiftsOrderQuery()
			{
				skey = skey,
				status = status,
				PageSize = rows,
				PageNo = page
			};
			PageModel<GiftOrderInfo> orders = orderser.GetOrders(giftsOrderQuery);
			List<GiftOrderInfo> list = orders.Models.ToList();
            orderser.OrderAddUserInfo(list);
			IEnumerable<GiftOrderPageModel> giftOrderPageModels = list.Select<GiftOrderInfo, GiftOrderPageModel>((GiftOrderInfo d) => {
				GiftOrderItemInfo giftOrderItemInfo = d.Himall_GiftOrderItem.FirstOrDefault();
				return new GiftOrderPageModel()
				{
					Id = d.Id,
					OrderStatus = d.OrderStatus,
					UserId = d.UserId,
					UserRemark = ClearHtmlString(d.UserRemark),
					ShipTo = d.ShipTo,
					CellPhone = d.CellPhone,
					TopRegionId = d.TopRegionId,
					RegionId = d.RegionId,
					RegionFullName = d.RegionFullName,
					Address = ClearHtmlString(d.Address),
					ExpressCompanyName = d.ExpressCompanyName,
					ShipOrderNumber = d.ShipOrderNumber,
					ShippingDate = d.ShippingDate,
					OrderDate = d.OrderDate,
					FinishDate = d.FinishDate,
					TotalIntegral = d.TotalIntegral,
					CloseReason = ClearHtmlString(d.CloseReason),
					FirstGiftId = giftOrderItemInfo.GiftId,
					FirstGiftName = ClearHtmlString(giftOrderItemInfo.GiftName),
					FirstGiftBuyQuantity = giftOrderItemInfo.Quantity,
					UserName = d.UserName
				};
			});
			var variable = new { rows = giftOrderPageModels.ToList(), total = orders.Total };
			return Json(variable);
		}

		[HttpPost]
		[OperationLog(Message="礼品订单发货")]
		public JsonResult SendGift(long id, string expname, string expnum)
		{
			Result result = new Result();
            orderser.SendGood(id, expname, expnum);
			result.success = true;
			result.status = 1;
			result.msg = "发货成功";
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult UpdateSequence(long id, int sequence)
		{
			ServiceHelper.Create<IGiftService>().UpdateSequence(id, sequence);
			return Json(new { success = true });
		}
	}
}