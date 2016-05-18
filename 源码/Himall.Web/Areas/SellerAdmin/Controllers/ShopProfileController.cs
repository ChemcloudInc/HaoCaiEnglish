using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class ShopProfileController : BaseMemberController
	{
		public ShopProfileController()
		{
		}

		[HttpPost]
		public ActionResult Agreement(FormCollection form)
		{
			if (!form["agree"].Equals("on"))
			{
				return RedirectToAction("EditProfile0");
			}
			ManagerInfo managerInfo = ServiceHelper.Create<IManagerService>().AddSellerManager(base.CurrentUser.UserName, base.CurrentUser.Password, base.CurrentUser.PasswordSalt);
			string str = UserCookieEncryptHelper.Encrypt(managerInfo.Id, "SellerAdmin");
			DateTime now = DateTime.Now;
			WebHelper.SetCookie("Himall-SellerManager", str, now.AddDays(7));
			return RedirectToAction("EditProfile1");
		}

		[HttpGet]
		public JsonResult CheckCompanyName(string companyName)
		{
			bool flag = ServiceHelper.Create<IShopService>().ExistCompanyName(companyName.Trim(), base.CurrentSellerManager.ShopId);
			return Json(!flag, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult EditProfile0()
		{
			ViewBag.Step = 0;
			ViewBag.MenuStep = 0;
			ViewBag.Frame = "Step0";
			ViewBag.Manager = base.CurrentUser;
			ViewBag.SellerAdminAgreement = GetSellerAgreement();
			return View("EditProfile");
		}

		[HttpGet]
		public ActionResult EditProfile1()
		{
			ViewBag.Step = 1;
			ViewBag.MenuStep = 1;
			ViewBag.Frame = "Step1";
			ViewBag.Manager = base.CurrentUser;
			ViewBag.SellerAdminAgreement = GetSellerAgreement();
			return View("EditProfile", new ShopProfileStep1());
		}

		[HttpPost]
		public JsonResult EditProfile1(ShopProfileStep1 shopProfileStep1)
		{
			ShopInfo shopInfo = new ShopInfo()
			{
				Id = base.CurrentSellerManager.ShopId,
				CompanyName = shopProfileStep1.CompanyName,
				CompanyAddress = shopProfileStep1.Address,
				CompanyRegionId = shopProfileStep1.CityRegionId,
				CompanyRegionAddress = shopProfileStep1.Address,
				CompanyPhone = shopProfileStep1.Phone,
				CompanyEmployeeCount = shopProfileStep1.EmployeeCount,
				CompanyRegisteredCapital = shopProfileStep1.RegisterMoney,
				ContactsName = shopProfileStep1.ContactName,
				ContactsPhone = shopProfileStep1.ContactPhone,
				ContactsEmail = shopProfileStep1.Email,
				BusinessLicenceNumber = shopProfileStep1.BusinessLicenceNumber,
				BusinessLicenceRegionId = shopProfileStep1.BusinessLicenceArea,
				BusinessLicenceStart = new DateTime?(shopProfileStep1.BusinessLicenceValidStart),
				BusinessLicenceEnd = new DateTime?(shopProfileStep1.BusinessLicenceValidEnd),
				BusinessSphere = shopProfileStep1.BusinessSphere,
				BusinessLicenceNumberPhoto = shopProfileStep1.BusinessLicenceNumberPhoto,
				OrganizationCode = shopProfileStep1.OrganizationCode,
				OrganizationCodePhoto = shopProfileStep1.OrganizationCodePhoto,
				GeneralTaxpayerPhot = shopProfileStep1.GeneralTaxpayerPhoto,
				Stage = new ShopInfo.ShopStage?(ShopInfo.ShopStage.FinancialInfo),
				BusinessLicenseCert = base.Request.Form["BusinessLicenseCert"],
				ProductCert = base.Request.Form["ProductCert"],
				OtherCert = base.Request.Form["OtherCert"],
				legalPerson = shopProfileStep1.legalPerson,
				CompanyFoundingDate = new DateTime?(shopProfileStep1.CompanyFoundingDate)
			};
			ServiceHelper.Create<IShopService>().UpdateShop(shopInfo);
			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult EditProfile2(ShopProfileStep2 shopProfileStep2)
		{
			ShopInfo shopInfo = new ShopInfo()
			{
				Id = base.CurrentSellerManager.ShopId,
				BankAccountName = shopProfileStep2.BankAccountName,
				BankAccountNumber = shopProfileStep2.BankAccountNumber,
				BankCode = shopProfileStep2.BankCode,
				BankName = shopProfileStep2.BankName,
				BankPhoto = shopProfileStep2.BankPhoto,
				BankRegionId = shopProfileStep2.BankRegionId,
				TaxpayerId = shopProfileStep2.TaxpayerId,
				TaxRegistrationCertificate = shopProfileStep2.TaxRegistrationCertificate,
				TaxRegistrationCertificatePhoto = shopProfileStep2.TaxRegistrationCertificatePhoto,
				Stage = new ShopInfo.ShopStage?(ShopInfo.ShopStage.ShopInfo)
			};
			ServiceHelper.Create<IShopService>().UpdateShop(shopInfo);
			return Json(new { success = true });
		}

		[HttpGet]
		public ActionResult EditProfile2()
		{
			ViewBag.MenuStep = 2;
			ViewBag.Step = 1;
			ViewBag.Frame = "Step2";
			ViewBag.Manager = base.CurrentUser;
			ViewBag.SellerAdminAgreement = GetSellerAgreement();
			return View("EditProfile", new ShopProfileStep2());
		}

		[HttpGet]
		public ActionResult EditProfile3()
		{
			ViewBag.Step = 1;
			ViewBag.Frame = "Step3";
			ViewBag.Manager = base.CurrentUser;
			ViewBag.SellerAdminAgreement = GetSellerAgreement();
			return View("EditProfile");
		}

		[HttpPost]
		public JsonResult EditProfile3(string shopProfileStep3)
		{
			ShopProfileStep3 shopProfileStep31 = JsonConvert.DeserializeObject<ShopProfileStep3>(shopProfileStep3);
			if (ServiceHelper.Create<IShopService>().ExistShop(shopProfileStep31.ShopName, base.CurrentSellerManager.ShopId))
			{
				string str = string.Format("{0} 店铺名称已经存在", shopProfileStep31.ShopName);
				return Json(new { success = false, msg = str });
			}
			ShopInfo shopInfo = new ShopInfo()
			{
				Id = base.CurrentSellerManager.ShopId,
				ShopName = shopProfileStep31.ShopName,
				GradeId = shopProfileStep31.ShopGrade,
				Stage = new ShopInfo.ShopStage?(ShopInfo.ShopStage.UploadPayOrder)
			};
			ShopInfo shopInfo1 = shopInfo;
			IEnumerable<long> categories = shopProfileStep31.Categories;
			ServiceHelper.Create<IShopService>().UpdateShop(shopInfo1, categories);
			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult EditProfile4(string payOrderPhoto, string remark)
		{
			ShopInfo shopInfo = new ShopInfo()
			{
				Id = base.CurrentSellerManager.ShopId,
				PayPhoto = payOrderPhoto,
				PayRemark = remark,
				Stage = new ShopInfo.ShopStage?(ShopInfo.ShopStage.Finish),
				ShopStatus = ShopInfo.ShopAuditStatus.WaitConfirm
			};
			ServiceHelper.Create<IShopService>().UpdateShop(shopInfo);
			return Json(new { success = true });
		}

		[HttpGet]
		public ActionResult EditProfile4()
		{
			ViewBag.Step = 2;
			ViewBag.Frame = "Step4";
			ViewBag.Manager = base.CurrentUser;
			ViewBag.Username = base.CurrentSellerManager.UserName;
			ViewBag.SellerAdminAgreement = GetSellerAgreement();
			return View("EditProfile");
		}

		[HttpGet]
		public ActionResult EditProfile5()
		{
			ViewBag.Step = 3;
			ViewBag.Frame = "Step5";
			ViewBag.Manager = base.CurrentUser;
			return View("EditProfile");
		}

		[HttpGet]
		public ActionResult EditProfile6()
		{
			ViewBag.Step = 2;
			ViewBag.Frame = "Step6";
			ViewBag.Manager = base.CurrentUser;
			ViewBag.SellerAdminAgreement = GetSellerAgreement();
			return View("EditProfile");
		}

		public ActionResult Finish()
		{
			return View();
		}

		[HttpPost]
		public JsonResult GetCategories(long? key = null, int? level = -1)
		{
			IEnumerable<CategoryInfo> validBusinessCategoryByParentId = ServiceHelper.Create<ICategoryService>().GetValidBusinessCategoryByParentId(key.GetValueOrDefault());
			IEnumerable<KeyValuePair<long, string>> keyValuePair = 
				from item in validBusinessCategoryByParentId
				select new KeyValuePair<long, string>(item.Id, item.Name);
			return Json(keyValuePair);
		}

		public string GetSellerAgreement()
		{
			AgreementInfo agreement = ServiceHelper.Create<ISystemAgreementService>().GetAgreement(AgreementInfo.AgreementTypes.Seller);
			if (agreement == null)
			{
				return "";
			}
			return agreement.AgreementContent;
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.Logo = base.CurrentSiteSetting.MemberLogo;
			filterContext.RouteData.Values["controller"].ToString().ToLower();
			string lower = filterContext.RouteData.Values["action"].ToString().ToLower();
			filterContext.RouteData.DataTokens["area"].ToString().ToLower();
			Log.Info(string.Concat("Executing:", lower));
			if (base.CurrentSellerManager == null && lower.IndexOf("step") != 0 && filterContext.RequestContext.HttpContext.Request.HttpMethod.ToUpper() != "POST")
			{
				if (lower != "EditProfile0".ToLower())
				{
					RedirectToRouteResult action = RedirectToAction("EditProfile0", "ShopProfile", new { area = "SellerAdmin" });
					filterContext.Result = action;
					Log.Info(string.Concat("Executing1:", lower));
					return;
				}
			}
			else if (base.CurrentSellerManager != null)
			{
				ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, false);
				int valueOrDefault = (int)shop.Stage.GetValueOrDefault();
				ShopInfo.ShopStage? stage = shop.Stage;
				if ((stage.GetValueOrDefault() != ShopInfo.ShopStage.Finish ? false : stage.HasValue) && shop.ShopStatus == ShopInfo.ShopAuditStatus.Open)
				{
					RedirectToRouteResult redirectToRouteResult = RedirectToAction("index", "home", new { area = "SellerAdmin" });
					filterContext.Result = redirectToRouteResult;
					Log.Info(string.Concat("Executing2:", lower));
					return;
				}
				ShopInfo.ShopStage? nullable = shop.Stage;
				if (((nullable.GetValueOrDefault() != ShopInfo.ShopStage.Finish ? true : !nullable.HasValue) || shop.ShopStatus == ShopInfo.ShopAuditStatus.WaitConfirm) && filterContext.RequestContext.HttpContext.Request.HttpMethod.ToUpper() != "POST" && lower.IndexOf("step") != 0)
				{
					bool flag = true;
					if (shop.ShopStatus == ShopInfo.ShopAuditStatus.Refuse || shop.ShopStatus == ShopInfo.ShopAuditStatus.Unusable)
					{
						string str = lower.ToLower().Replace("EditProfile".ToLower(), "");
						int num = 0;
						if (int.TryParse(str, out num) && num >= 1 && num <= 3)
						{
							flag = false;
						}
					}
					if (flag && lower != string.Concat("EditProfile", valueOrDefault).ToLower())
					{
						RedirectToRouteResult action1 = RedirectToAction(string.Concat("EditProfile", valueOrDefault), "ShopProfile", new { area = "SellerAdmin" });
						filterContext.Result = action1;
						Log.Info(string.Concat("Executing3:", lower));
					}
				}
			}
		}

		[HttpGet]
		public ActionResult Step0()
		{
			ViewBag.SellerAdminAgreement = GetSellerAgreement();
			return View();
		}

		[HttpGet]
		public ActionResult Step1()
		{
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, false);
			ShopProfileStep1 shopProfileStep1 = new ShopProfileStep1()
			{
				Address = shop.CompanyAddress,
				BusinessLicenceArea = shop.BusinessLicenceRegionId,
				BusinessLicenceNumber = shop.BusinessLicenceNumber,
				BusinessLicenceNumberPhoto = shop.BusinessLicenceNumberPhoto
			};
			if (shop.BusinessLicenceEnd.HasValue)
			{
				shopProfileStep1.BusinessLicenceValidEnd = shop.BusinessLicenceEnd.Value;
			}
			if (shop.BusinessLicenceStart.HasValue)
			{
				shopProfileStep1.BusinessLicenceValidStart = shop.BusinessLicenceStart.Value;
			}
			string empty = string.Empty;
			for (int i = 1; i < 4; i++)
			{
				if (System.IO.File.Exists(Server.MapPath(string.Concat(shop.BusinessLicenseCert, string.Format("{0}.png", i)))))
				{
					empty = string.Concat(empty, shop.BusinessLicenseCert, string.Format("{0}.png", i), ",");
				}
			}
			char[] chrArray = new char[] { ',' };
			shopProfileStep1.BusinessLicenseCert = empty.TrimEnd(chrArray);
			shopProfileStep1.BusinessSphere = shop.BusinessSphere;
			shopProfileStep1.CityRegionId = shop.CompanyRegionId;
			if (shop.CompanyFoundingDate.HasValue)
			{
				shopProfileStep1.CompanyFoundingDate = shop.CompanyFoundingDate.Value;
			}
			shopProfileStep1.CompanyName = shop.CompanyName;
			shopProfileStep1.ContactName = shop.ContactsName;
			shopProfileStep1.ContactPhone = shop.ContactsPhone;
			shopProfileStep1.Email = shop.ContactsEmail;
			shopProfileStep1.EmployeeCount = shop.CompanyEmployeeCount;
			shopProfileStep1.GeneralTaxpayerPhoto = shop.GeneralTaxpayerPhot;
			shopProfileStep1.legalPerson = shop.legalPerson;
			shopProfileStep1.OrganizationCode = shop.OrganizationCode;
			shopProfileStep1.OrganizationCodePhoto = shop.OrganizationCodePhoto;
			string str = string.Empty;
			for (int j = 1; j < 4; j++)
			{
				if (System.IO.File.Exists(Server.MapPath(string.Concat(shop.OtherCert, string.Format("{0}.png", j)))))
				{
					str = string.Concat(str, shop.OtherCert, string.Format("{0}.png", j), ",");
				}
			}
			char[] chrArray1 = new char[] { ',' };
			shopProfileStep1.OtherCert = str.TrimEnd(chrArray1);
			shopProfileStep1.Phone = shop.CompanyPhone;
			string empty1 = string.Empty;
			for (int k = 1; k < 4; k++)
			{
				if (System.IO.File.Exists(Server.MapPath(string.Concat(shop.ProductCert, string.Format("{0}.png", k)))))
				{
					empty1 = string.Concat(empty1, shop.ProductCert, string.Format("{0}.png", k), ",");
				}
			}
			char[] chrArray2 = new char[] { ',' };
			shopProfileStep1.ProductCert = empty1.TrimEnd(chrArray2);
			shopProfileStep1.RegisterMoney = shop.CompanyRegisteredCapital;
			shopProfileStep1.taxRegistrationCert = shop.TaxRegistrationCertificate;
			ViewBag.CompanyRegionIds = ServiceHelper.Create<IRegionService>().GetRegionIdPath(shop.CompanyRegionId);
			ViewBag.BusinessLicenceRegionIds = ServiceHelper.Create<IRegionService>().GetRegionIdPath(shop.BusinessLicenceRegionId);
			string refuseReason = "";
			if (shop.ShopStatus == ShopInfo.ShopAuditStatus.Refuse)
			{
				refuseReason = shop.RefuseReason;
			}
			ViewBag.RefuseReason = refuseReason;
			return View(shopProfileStep1);
		}

		[HttpGet]
		public ActionResult Step2()
		{
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, false);
			ShopProfileStep2 shopProfileStep2 = new ShopProfileStep2()
			{
				BankAccountName = shop.BankAccountName,
				BankAccountNumber = shop.BankAccountNumber,
				BankCode = shop.BankCode,
				BankName = shop.BankName,
				BankPhoto = shop.BankPhoto,
				BankRegionId = shop.BankRegionId,
				TaxpayerId = shop.TaxpayerId,
				TaxRegistrationCertificate = shop.TaxRegistrationCertificate,
				TaxRegistrationCertificatePhoto = shop.TaxRegistrationCertificatePhoto
			};
			ViewBag.BankRegionIds = ServiceHelper.Create<IRegionService>().GetRegionIdPath(shop.BankRegionId);
			return View(shopProfileStep2);
		}

		[HttpGet]
		public ActionResult Step3()
		{
			ViewBag.ShopGrades = ServiceHelper.Create<IShopService>().GetShopGrades();
			ViewBag.ShopCategories = ServiceHelper.Create<ICategoryService>().GetMainCategory();
			ViewBag.Username = base.CurrentSellerManager.UserName;
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, true);
			List<CategoryKeyVal> categoryKeyVals = new List<CategoryKeyVal>();
			foreach (long key in shop.BusinessCategory.Keys)
			{
				CategoryKeyVal categoryKeyVal = new CategoryKeyVal()
				{
					CommisRate = shop.BusinessCategory[key],
					Name = ServiceHelper.Create<ICategoryService>().GetCategory(key).Name
				};
				categoryKeyVals.Add(categoryKeyVal);
			}
			ShopProfileStep3 shopProfileStep3 = new ShopProfileStep3()
			{
				ShopName = shop.ShopName,
				ShopGrade = shop.GradeId,
				BusinessCategory = ServiceHelper.Create<IShopService>().GetBusinessCategory(base.CurrentSellerManager.ShopId).ToList()
			};
			return View(shopProfileStep3);
		}

		[HttpGet]
		public ActionResult Step4()
		{
			string str;
			ShopInfo shop = ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, true);
			if (shop.ShopStatus != ShopInfo.ShopAuditStatus.WaitPay)
			{
				if (shop.ShopStatus != ShopInfo.ShopAuditStatus.WaitAudit)
				{
					if (shop.ShopStatus != ShopInfo.ShopAuditStatus.Refuse)
					{
						return RedirectToAction("Finish");
					}
					ViewBag.Step = 1;
					ViewBag.MenuStep = 1;
					ViewBag.Frame = "Step1";
					ViewBag.Manager = base.CurrentSellerManager;
					return View("EditProfile", new ShopProfileStep1());
				}
				str = "step4";
				ViewBag.Text = "入驻申请已经提交，请等待管理员审核并准备提交付款凭证";
			}
			else
			{
				str = "step5";
			}
			ShopModel shopModel = new ShopModel(shop)
			{
				BusinessCategory = new List<CategoryKeyVal>()
			};
			foreach (long key in shop.BusinessCategory.Keys)
			{
				List<CategoryKeyVal> businessCategory = shopModel.BusinessCategory;
				CategoryKeyVal categoryKeyVal = new CategoryKeyVal()
				{
					CommisRate = shop.BusinessCategory[key],
					Name = ServiceHelper.Create<ICategoryService>().GetCategory(key).Name
				};
				businessCategory.Add(categoryKeyVal);
			}
			return View(str, shopModel);
		}

		[HttpGet]
		public ActionResult Step5()
		{
			ViewBag.Text = "付款凭证已经提交，请等待管理员核对后为您开通店铺";
			if (ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, false).ShopStatus == ShopInfo.ShopAuditStatus.WaitConfirm)
			{
				return View("step4");
			}
			return RedirectToAction("Finish");
		}
	}
}