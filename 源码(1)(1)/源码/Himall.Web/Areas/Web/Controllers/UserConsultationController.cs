using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class UserConsultationController : BaseMemberController
	{
		public UserConsultationController()
		{
		}

		public ActionResult Index(int pageNo = 1, int pageSize = 20)
		{
			ConsultationQuery consultationQuery = new ConsultationQuery()
			{
				UserID = base.CurrentUser.Id,
				PageNo = pageNo,
				PageSize = pageSize
			};
			PageModel<ProductConsultationInfo> consultations = ServiceHelper.Create<IConsultationService>().GetConsultations(consultationQuery);
			IEnumerable<ProductConsultationModel> list = 
				from item in consultations.Models.ToList()
				select new ProductConsultationModel()
				{
					Id = item.Id,
					ConsultationContent = item.ConsultationContent,
					ConsultationDate = item.ConsultationDate,
					ProductName = item.ProductInfo.ProductName,
					ProductPic = item.ProductInfo.ImagePath,
					ProductId = item.ProductId,
					UserName = item.UserName,
					ReplyContent = item.ReplyContent,
					ReplyDate = item.ReplyDate
				};
			PagingInfo pagingInfo = new PagingInfo()
			{
				CurrentPage = pageNo,
				ItemsPerPage = pageSize,
				TotalItems = consultations.Total
			};
			ViewBag.pageInfo = pagingInfo;
			return View(list);
		}
	}
}