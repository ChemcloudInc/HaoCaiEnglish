using Himall.Core;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class AccountController : BaseAdminController
	{
		public AccountController()
		{
		}

		public FileResult AgreementDetailListExportExcel(long accountId, int enumOrderTypeId, DateTime? startDate, DateTime? endDate)
		{
			AccountQuery accountQuery = new AccountQuery()
			{
				StartDate = startDate,
				EndDate = (endDate.HasValue ? new DateTime?(endDate.Value.AddDays(1)) : endDate),
				AccountId = accountId,
				PageSize = 2147483647,
				PageNo = 1
			};
			PageModel<AccountPurchaseAgreementInfo> accountPurchaseAgreements = ServiceHelper.Create<IAccountService>().GetAccountPurchaseAgreements(accountQuery);
			var list = (
				from p in accountPurchaseAgreements.Models.ToList()
				select new { Id = p.Id, OrderTypeDescription = "预付款列表", PurchaseAgreementId = p.PurchaseAgreementId, AdvancePayment = p.AdvancePayment, FinishDate = p.FinishDate.ToString(), ApplyDate = p.ApplyDate.ToString(), Date = p.Date.ToString() }).ToList();
			HSSFWorkbook hSSFWorkbook = new HSSFWorkbook();
			Sheet sheet = hSSFWorkbook.CreateSheet("Sheet1");
			Row row = sheet.CreateRow(0);
			row.CreateCell(0).SetCellValue("类型");
			row.CreateCell(1).SetCellValue("采购协议编号");
			row.CreateCell(2).SetCellValue("预付款金额");
			row.CreateCell(3).SetCellValue("申请结算时间");
			row.CreateCell(4).SetCellValue("通过审核时间");
			sheet.SetColumnWidth(0, 2750);
			sheet.SetColumnWidth(1, 11000);
			sheet.SetColumnWidth(2, 4400);
			sheet.SetColumnWidth(3, 8250);
			sheet.SetColumnWidth(4, 8250);
			for (int i = 0; i < list.Count(); i++)
			{
				Row row1 = sheet.CreateRow(i + 1);
				row1.CreateCell(0).SetCellValue(list[i].OrderTypeDescription);
				row1.CreateCell(1).SetCellValue(list[i].PurchaseAgreementId);
				row1.CreateCell(2).SetCellValue(list[i].AdvancePayment.ToString());
				row1.CreateCell(3).SetCellValue(list[i].ApplyDate.ToString());
				row1.CreateCell(4).SetCellValue(list[i].FinishDate.ToString());
			}
			MemoryStream memoryStream = new MemoryStream();
			hSSFWorkbook.Write(memoryStream);
			memoryStream.Seek(0, SeekOrigin.Begin);
			return File(memoryStream, "application/vnd.ms-excel", "结算详情-预付款列表.xls");
		}

		[HttpPost]
		[OperationLog(Message="确认结算")]
		[UnAuthorize]
		public JsonResult ConfirmAccount(long id, string remark)
		{
			Result result = new Result();
			try
			{
				ServiceHelper.Create<IAccountService>().ConfirmAccount(id, remark);
				result.success = true;
			}
			catch (Exception exception)
			{
				result.msg = exception.Message;
			}
			return Json(result);
		}

		[UnAuthorize]
		public ActionResult Detail(long id)
		{
			return View(ServiceHelper.Create<IAccountService>().GetAccount(id));
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
				PageNo = page
			};
			PageModel<AccountDetailInfo> accountDetails = ServiceHelper.Create<IAccountService>().GetAccountDetails(accountQuery);
			var list = 
				from p in accountDetails.Models.ToList()
				select new { Id = p.Id, OrderType = p.OrderType, OrderTypeDescription = p.OrderType.ToDescription(), OrderId = p.OrderId, ProductActualPaidAmount = p.ProductActualPaidAmount, FreightAmount = p.FreightAmount, CommissionAmount = p.CommissionAmount, RefundCommisAmount = p.RefundCommisAmount, RefundTotalAmount = p.RefundTotalAmount, Date = p.Date.ToString(), OrderDate = p.OrderDate.ToString(), OrderRefundsDates = p.OrderRefundsDates };
			return Json(new { rows = list, total = accountDetails.Total });
		}

		public FileResult DetailListExportExcel(long accountId, int enumOrderTypeId, DateTime? startDate, DateTime? endDate)
		{
			AccountQuery accountQuery = new AccountQuery()
			{
				StartDate = startDate,
				EndDate = (endDate.HasValue ? new DateTime?(endDate.Value.AddDays(1)) : endDate),
				AccountId = accountId,
				PageSize = 2147483647,
				EnumOrderType = (AccountDetailInfo.EnumOrderType)enumOrderTypeId,
				PageNo = 1
			};
			PageModel<AccountDetailInfo> accountDetails = ServiceHelper.Create<IAccountService>().GetAccountDetails(accountQuery);
			var list = (
				from p in accountDetails.Models.ToList()
				select new { Id = p.Id, OrderType = p.OrderType, OrderTypeDescription = p.OrderType.ToDescription(), OrderId = p.OrderId, ProductActualPaidAmount = p.ProductActualPaidAmount, FreightAmount = p.FreightAmount, CommissionAmount = p.CommissionAmount, RefundCommisAmount = p.RefundCommisAmount, RefundTotalAmount = p.RefundTotalAmount, Date = p.Date.ToString(), OrderDate = p.OrderDate.ToString(), OrderRefundsDates = p.OrderRefundsDates }).ToList();
			HSSFWorkbook hSSFWorkbook = new HSSFWorkbook();
			Sheet sheet = hSSFWorkbook.CreateSheet("Sheet1");
			string empty = string.Empty;
			Row row = sheet.CreateRow(0);
			if (enumOrderTypeId == 1)
			{
				empty = "订单列表";
				row.CreateCell(0).SetCellValue("类型");
				row.CreateCell(1).SetCellValue("订单编号");
				row.CreateCell(2).SetCellValue("商品实付金额");
				row.CreateCell(3).SetCellValue("运费");
				row.CreateCell(4).SetCellValue("佣金");
				row.CreateCell(5).SetCellValue("下单日期");
				row.CreateCell(6).SetCellValue("成交日期");
				sheet.SetColumnWidth(0, 2750);
				sheet.SetColumnWidth(1, 11000);
				sheet.SetColumnWidth(2, 4400);
				sheet.SetColumnWidth(3, 2750);
				sheet.SetColumnWidth(4, 2750);
				sheet.SetColumnWidth(5, 8250);
				sheet.SetColumnWidth(6, 8250);
			}
			else if (enumOrderTypeId == 0)
			{
				empty = "退单列表";
				row.CreateCell(0).SetCellValue("类型");
				row.CreateCell(1).SetCellValue("订单编号");
				row.CreateCell(2).SetCellValue("商品实付金额");
				row.CreateCell(3).SetCellValue("运费");
				row.CreateCell(4).SetCellValue("退款金额");
				row.CreateCell(5).SetCellValue("退还佣金");
				row.CreateCell(6).SetCellValue("退单日期");
				sheet.SetColumnWidth(0, 2750);
				sheet.SetColumnWidth(1, 11000);
				sheet.SetColumnWidth(2, 4400);
				sheet.SetColumnWidth(3, 2750);
				sheet.SetColumnWidth(4, 4400);
				sheet.SetColumnWidth(5, 2750);
				sheet.SetColumnWidth(6, 8250);
			}
			for (int i = 0; i < list.Count(); i++)
			{
				Row row1 = sheet.CreateRow(i + 1);
				if (enumOrderTypeId == 1)
				{
					row1.CreateCell(0).SetCellValue(empty);
					row1.CreateCell(1).SetCellValue(list[i].OrderId.ToString());
					row1.CreateCell(2).SetCellValue(list[i].ProductActualPaidAmount.ToString());
					row1.CreateCell(3).SetCellValue(list[i].FreightAmount.ToString());
					row1.CreateCell(4).SetCellValue(list[i].CommissionAmount.ToString());
					row1.CreateCell(5).SetCellValue(list[i].OrderDate.ToString());
					row1.CreateCell(6).SetCellValue(list[i].Date.ToString());
				}
				else if (enumOrderTypeId == 0)
				{
					row1.CreateCell(0).SetCellValue(empty);
					row1.CreateCell(1).SetCellValue(list[i].OrderId.ToString());
					row1.CreateCell(2).SetCellValue(list[i].ProductActualPaidAmount.ToString());
					row1.CreateCell(3).SetCellValue(list[i].FreightAmount.ToString());
					row1.CreateCell(4).SetCellValue(list[i].RefundTotalAmount.ToString());
					row1.CreateCell(5).SetCellValue(list[i].RefundCommisAmount.ToString());
					row1.CreateCell(6).SetCellValue(list[i].OrderRefundsDates.ToString());
				}
			}
			MemoryStream memoryStream = new MemoryStream();
			hSSFWorkbook.Write(memoryStream);
			memoryStream.Seek(0, SeekOrigin.Begin);
			return File(memoryStream, "application/vnd.ms-excel", string.Format("结算详情-{0}.xls", empty));
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult ExecutSettlement()
		{
			return Json(new { success = true });
		}

		public FileResult ExportExcel(int status, string shopName)
		{
			AccountQuery accountQuery = new AccountQuery()
			{
				Status = new AccountInfo.AccountStatus?((AccountInfo.AccountStatus)status),
				ShopName = shopName,
				PageSize = 2147483647,
				PageNo = 1
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
					AccountDate = accountInfo.AccountDate.ToString(),
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
			HSSFWorkbook hSSFWorkbook = new HSSFWorkbook();
			Sheet sheet = hSSFWorkbook.CreateSheet("Sheet1");
			Row row = sheet.CreateRow(0);
			row.CreateCell(0).SetCellValue("店铺名称");
			row.CreateCell(1).SetCellValue("时间段");
			row.CreateCell(2).SetCellValue("商品实付总额");
			row.CreateCell(3).SetCellValue("运费");
			row.CreateCell(4).SetCellValue("佣金");
			row.CreateCell(5).SetCellValue("退款金额");
			row.CreateCell(6).SetCellValue("退还佣金");
			row.CreateCell(7).SetCellValue("营销费用总额");
			row.CreateCell(8).SetCellValue("本期应结");
			row.CreateCell(9).SetCellValue("出账日期");
			for (int j = 0; j < accountModels.Count; j++)
			{
				Row row1 = sheet.CreateRow(j + 1);
				row1.CreateCell(0).SetCellValue(accountModels[j].ShopName);
				row1.CreateCell(1).SetCellValue(accountModels[j].TimeSlot);
				row1.CreateCell(2).SetCellValue(accountModels[j].ProductActualPaidAmount.ToString());
				row1.CreateCell(3).SetCellValue(accountModels[j].FreightAmount.ToString());
				row1.CreateCell(4).SetCellValue(accountModels[j].CommissionAmount.ToString());
				row1.CreateCell(5).SetCellValue(accountModels[j].RefundAmount.ToString());
				row1.CreateCell(6).SetCellValue(accountModels[j].RefundCommissionAmount.ToString());
				row1.CreateCell(7).SetCellValue(accountModels[j].AdvancePaymentAmount.ToString());
				row1.CreateCell(8).SetCellValue(accountModels[j].PeriodSettlement.ToString());
				row1.CreateCell(9).SetCellValue(accountModels[j].AccountDate.ToString());
			}
			MemoryStream memoryStream = new MemoryStream();
			hSSFWorkbook.Write(memoryStream);
			memoryStream.Seek(0, SeekOrigin.Begin);
			return File(memoryStream, "application/vnd.ms-excel", "结算管理.xls");
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult List(int status, string shopName, int page, int rows)
		{
			AccountQuery accountQuery = new AccountQuery()
			{
				Status = new AccountInfo.AccountStatus?((AccountInfo.AccountStatus)status),
				ShopName = shopName,
				PageSize = rows,
				PageNo = page
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
					AccountDate = accountInfo.AccountDate.ToString(),
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

		public ActionResult SetSettlementWeek()
		{
			WeekSettlementModel weekSettlementModel = new WeekSettlementModel()
			{
				CurrentWeekSettlement = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings().WeekSettlement
			};
			return View(weekSettlementModel);
		}

		[HttpPost]
		public ActionResult UpdateSettlementWeek(WeekSettlementModel weekSettlementModel)
		{
			ServiceHelper.Create<ISiteSettingService>().SaveSetting("WeekSettlement", weekSettlementModel.NewWeekSettlement);
			return RedirectToAction("SetSettlementWeek");
		}
	}
}