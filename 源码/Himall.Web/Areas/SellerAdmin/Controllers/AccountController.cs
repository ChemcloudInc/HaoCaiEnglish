using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class AccountController : BaseSellerController
	{
		public AccountController()
		{
		}

		public ActionResult AccountDetail(long id)
		{
			AccountInfo account = ServiceHelper.Create<IAccountService>().GetAccount(id);
			if (account.ShopId != base.CurrentSellerManager.ShopId)
			{
				throw new HimallException(string.Concat("不存在该结算信息", id));
			}
			return View(account);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult DetailList(long accountId, int enumOrderTypeId, DateTime? startDate, DateTime? endDate, int page, int rows)
		{
			AccountQuery accountQuery = new AccountQuery()
			{
				StartDate = startDate,
				EndDate = (endDate.HasValue ? new DateTime?(endDate.Value.AddDays(1)) : endDate),
				AccountId = accountId,
				PageSize = rows,
				EnumOrderType = (AccountDetailInfo.EnumOrderType)enumOrderTypeId,
				PageNo = page,
				ShopId = new long?(base.CurrentSellerManager.ShopId)
			};
			PageModel<AccountDetailInfo> accountDetails = ServiceHelper.Create<IAccountService>().GetAccountDetails(accountQuery);
			var list = 
				from p in accountDetails.Models.ToList()
				select new { Id = p.Id, OrderType = p.OrderType, OrderTypeDescription = p.OrderType.ToDescription(), OrderId = p.OrderId, ProductActualPaidAmount = p.ProductActualPaidAmount, FreightAmount = p.FreightAmount, CommissionAmount = p.CommissionAmount, RefundCommisAmount = p.RefundCommisAmount, RefundTotalAmount = p.RefundTotalAmount, Date = p.Date.ToString(), OrderDate = p.OrderDate.ToString(), OrderRefundsDates = p.OrderRefundsDates };
			return Json(new { rows = list, total = accountDetails.Total });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(int status, int page, int rows)
		{
			AccountQuery accountQuery = new AccountQuery()
			{
				Status = new AccountInfo.AccountStatus?((AccountInfo.AccountStatus)status),
				PageSize = rows,
				PageNo = page,
				ShopId = new long?(base.CurrentSellerManager.ShopId)
			};
			PageModel<AccountInfo> accounts = ServiceHelper.Create<IAccountService>().GetAccounts(accountQuery);
			IList<AccountModel> accountModels = new List<AccountModel>();
			AccountInfo[] array = accounts.Models.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				AccountInfo accountInfo = array[i];
				AccountModel accountModel = new AccountModel()
				{
					Id = accountInfo.Id,
					ShopId = accountInfo.ShopId,
					ShopName = accountInfo.ShopName,
					AccountDate = accountInfo.AccountDate.ToLocalTime().ToString(),
					StartDate = accountInfo.StartDate,
					EndDate = accountInfo.EndDate,
					Status = (int)accountInfo.Status,
					ProductActualPaidAmount = accountInfo.ProductActualPaidAmount,
					FreightAmount = accountInfo.FreightAmount,
					CommissionAmount = accountInfo.CommissionAmount,
					RefundAmount = accountInfo.RefundAmount,
					RefundCommissionAmount = accountInfo.RefundCommissionAmount,
					AdvancePaymentAmount = accountInfo.AdvancePaymentAmount,
					PeriodSettlement = accountInfo.PeriodSettlement,
					Remark = accountInfo.Remark
				};
				string str = accountModel.StartDate.Date.ToString("yyyy-MM-dd");
				DateTime date = accountModel.EndDate.Date;
				accountModel.TimeSlot = string.Format("{0} 至 {1}", str, date.ToString("yyyy-MM-dd"));
				accountModels.Add(accountModel);
			}
			return Json(new { rows = accountModels, total = accounts.Total });
		}

		public ActionResult Management()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult MetaDetailList(long accountId, int enumOrderTypeId, DateTime? startDate, DateTime? endDate, int page, int rows)
		{
			AccountQuery accountQuery = new AccountQuery()
			{
				StartDate = startDate,
				EndDate = (endDate.HasValue ? new DateTime?(endDate.Value.AddDays(1)) : endDate),
				AccountId = accountId,
				PageSize = rows,
				PageNo = page
			};
			PageModel<AccountMetaModel> accountMeta = ServiceHelper.Create<IAccountService>().GetAccountMeta(accountQuery);
			IEnumerable<AccountMetaModel> list = 
				from e in accountMeta.Models.ToList()
				select new AccountMetaModel()
				{
					AccountId = e.Id,
					Id = e.Id,
					EndDate = e.EndDate,
					StartDate = e.StartDate,
					MetaKey = e.MetaKey,
					MetaValue = e.MetaValue,
					DateRange = string.Concat(e.StartDate.ToString("yyyy-MM-dd"), " 至 ", e.EndDate.ToString("yyyy-MM-dd"))
				};
			return Json(new { rows = list, total = accountMeta.Total });
		}
	}
}