using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class ProductConsultationController : BaseWebController
	{
		public ProductConsultationController()
		{
		}

		[HttpPost]
		public JsonResult AddConsultation(string Content, long productId = 0L)
		{
			if (productId == 0)
			{
				Result result = new Result()
				{
					success = false,
					msg = "咨询失败，该商品不存在或已经删除！"
				};
				return Json(result);
			}
			if (base.CurrentUser == null)
			{
				Result result1 = new Result()
				{
					success = false,
					msg = "登录超时，请重新登录！"
				};
				return Json(result1);
			}
			ProductConsultationInfo productConsultationInfo = new ProductConsultationInfo()
			{
				ConsultationContent = Content,
				ConsultationDate = DateTime.Now,
				ProductId = productId,
				UserId = base.CurrentUser.Id,
				UserName = base.CurrentUser.UserName,
				Email = base.CurrentUser.Email
			};
			ServiceHelper.Create<IConsultationService>().AddConsultation(productConsultationInfo);
			Result result2 = new Result()
			{
				success = true,
				msg = "咨询成功"
			};
			return Json(result2);
		}

		public ActionResult Index(long id = 0L)
		{
			decimal? nullable = ServiceHelper.Create<ICommentService>().GetCommentsByProductId(id).Average<ProductCommentInfo>((ProductCommentInfo a) => (decimal?)a.ReviewMark);
			decimal valueOrDefault = nullable.GetValueOrDefault();
			ViewBag.productMark = valueOrDefault;
			return View(ServiceHelper.Create<IProductService>().GetProduct(id));
		}
	}
}