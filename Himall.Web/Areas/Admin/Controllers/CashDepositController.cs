using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class CashDepositController : BaseAdminController
	{
		public CashDepositController()
		{
		}

		public ActionResult CashDepositDetail(long id)
		{
			ViewBag.Id = id;
			return View();
		}

		[HttpPost]
		public JsonResult CashDepositDetailList(long id, string name, DateTime? startDate, DateTime? endDate, int page, int rows)
		{
			CashDepositDetailQuery cashDepositDetailQuery = new CashDepositDetailQuery()
			{
				CashDepositId = id,
				Operator = name,
				StartDate = startDate,
				EndDate = endDate,
				PageNo = page,
				PageSize = rows
			};
			PageModel<CashDepositDetailInfo> cashDepositDetails = ServiceHelper.Create<ICashDepositsService>().GetCashDepositDetails(cashDepositDetailQuery);
			var array = 
				from item in cashDepositDetails.Models.ToArray()
                select new { Id = item.Id, Date = item.AddDate.ToString("yyyy-MM-dd HH:mm"), Balance = item.Balance, Operator = item.Operator, Description = item.Description };
			return Json(new { rows = array, total = cashDepositDetails.Total });
		}

		public ActionResult CashDepositRule()
		{
			List<CategoryCashDepositInfo> list = ServiceHelper.Create<ICashDepositsService>().GetCategoryCashDeposits().ToList();
			return View(list);
		}

		public JsonResult CloseNoReasonReturn(long categoryId)
		{
			ServiceHelper.Create<ICashDepositsService>().CloseNoReasonReturn(categoryId);
			return Json(new { Success = true });
		}

		public JsonResult Deduction(long id, string balance, string description)
		{
			if (Convert.ToDecimal(balance) < new decimal(0))
			{
				throw new HimallException("扣除保证金不能为负值");
			}
			CashDepositDetailInfo cashDepositDetailInfo = new CashDepositDetailInfo()
			{
				AddDate = DateTime.Now,
				Balance = -Convert.ToDecimal(balance),
				CashDepositId = id,
				Description = description,
				Operator = base.CurrentManager.UserName
			};
			ServiceHelper.Create<ICashDepositsService>().AddCashDepositDetails(cashDepositDetailInfo);
			return Json(new { Success = true });
		}

		[HttpPost]
		public JsonResult List(string shopName, int type, int page, int rows)
		{
			CashDepositQuery cashDepositQuery = new CashDepositQuery()
			{
				ShopName = shopName,
				PageNo = page,
				PageSize = rows
			};
			CashDepositQuery nullable = cashDepositQuery;
			if (type == 1)
			{
				nullable.Type = new bool?(true);
			}
			if (type == 2)
			{
				nullable.Type = new bool?(false);
			}
			ICashDepositsService cashDepositsService = ServiceHelper.Create<ICashDepositsService>();
			PageModel<CashDepositInfo> cashDeposits = cashDepositsService.GetCashDeposits(nullable);
			var collection = cashDeposits.Models.ToArray().Select((CashDepositInfo item) => {
				decimal needPayCashDepositByShopId = cashDepositsService.GetNeedPayCashDepositByShopId(item.ShopId);
				return new { Id = item.Id, ShopName = item.Himall_Shops.ShopName, Type = (needPayCashDepositByShopId > new decimal(0) ? "欠费" : "正常"), TotalBalance = item.TotalBalance, CurrentBalance = item.CurrentBalance, Date = item.Date.ToString("yyyy-MM-dd HH:mm"), NeedPay = needPayCashDepositByShopId, EnableLabels = item.EnableLabels };
			});
			return Json(new { rows = collection, total = cashDeposits.Total });
		}

		public ActionResult Management()
		{
			return View();
		}

		public JsonResult OpenNoReasonReturn(long categoryId)
		{
			ServiceHelper.Create<ICashDepositsService>().OpenNoReasonReturn(categoryId);
			return Json(new { Success = true });
		}

		public JsonResult UpdateEnableLabels(long id, bool enableLabels)
		{
			Instance<ICashDepositsService>.Create.UpdateEnableLabels(id, enableLabels);
			return Json(new { Success = true });
		}

		public JsonResult UpdateNeedPayCashDeposit(long categoryId, decimal cashDeposit)
		{
			ServiceHelper.Create<ICashDepositsService>().UpdateNeedPayCashDeposit(categoryId, cashDeposit);
			return Json(new { Success = true });
		}
	}
}