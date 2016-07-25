using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using System;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class AgreementController : BaseAdminController
	{
		public AgreementController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetManagement(int agreementType)
		{
			return Json(GetManagementModel((AgreementInfo.AgreementTypes)agreementType));
		}

		[UnAuthorize]
		public AgreementModel GetManagementModel(AgreementInfo.AgreementTypes type)
		{
			AgreementModel agreementModel = new AgreementModel();
			AgreementInfo agreement = ServiceHelper.Create<ISystemAgreementService>().GetAgreement(type);
			agreementModel.AgreementType = agreement.AgreementType;
			agreementModel.AgreementContent = agreement.AgreementContent;
			return agreementModel;
		}

		[UnAuthorize]
		public ActionResult Management()
		{
			return View(GetManagementModel(AgreementInfo.AgreementTypes.Buyers));
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult UpdateAgreement(int agreementType, string agreementContent)
		{
			ISystemAgreementService systemAgreementService = ServiceHelper.Create<ISystemAgreementService>();
			AgreementInfo agreement = systemAgreementService.GetAgreement((AgreementInfo.AgreementTypes)agreementType);
			agreement.AgreementType = agreementType;
			agreement.AgreementContent = agreementContent;
			if (systemAgreementService.UpdateAgreement(agreement))
			{
				Result result = new Result()
				{
					success = true,
					msg = "更新协议成功！"
				};
				return Json(result);
			}
			Result result1 = new Result()
			{
				success = false,
				msg = "更新协议失败！"
			};
			return Json(result1);
		}
	}
}