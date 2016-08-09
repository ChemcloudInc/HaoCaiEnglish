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
	public class ProductCommentController : BaseController
	{
		public ProductCommentController()
		{
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