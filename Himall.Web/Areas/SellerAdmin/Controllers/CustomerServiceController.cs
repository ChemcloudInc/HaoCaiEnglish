using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class CustomerServiceController : BaseSellerController
	{
		public CustomerServiceController()
		{
		}

		public ActionResult Add(long? id)
		{
			CustomerServiceInfo customerServiceInfo;
			CustomerServiceModel customerServiceModel;
			ICustomerService customerService = ServiceHelper.Create<ICustomerService>();
			if (id.HasValue)
			{
				long? nullable = id;
				if ((nullable.GetValueOrDefault() <= 0 ? true : !nullable.HasValue))
				{
					customerServiceInfo = new CustomerServiceInfo();
					customerServiceModel = new CustomerServiceModel()
					{
						Id = customerServiceInfo.Id,
						Account = customerServiceInfo.AccountCode,
						Name = customerServiceInfo.Name,
						Tool = customerServiceInfo.Tool,
						Type = new CustomerServiceInfo.ServiceType?(customerServiceInfo.Type)
					};
					return View(customerServiceModel);
				}
				customerServiceInfo = customerService.GetCustomerService(base.CurrentSellerManager.ShopId, id.Value);
				customerServiceModel = new CustomerServiceModel()
				{
					Id = customerServiceInfo.Id,
					Account = customerServiceInfo.AccountCode,
					Name = customerServiceInfo.Name,
					Tool = customerServiceInfo.Tool,
					Type = new CustomerServiceInfo.ServiceType?(customerServiceInfo.Type)
				};
				return View(customerServiceModel);
			}
			customerServiceInfo = new CustomerServiceInfo();
			customerServiceModel = new CustomerServiceModel()
			{
				Id = customerServiceInfo.Id,
				Account = customerServiceInfo.AccountCode,
				Name = customerServiceInfo.Name,
				Tool = customerServiceInfo.Tool,
				Type = new CustomerServiceInfo.ServiceType?(customerServiceInfo.Type)
			};
			return View(customerServiceModel);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Add(CustomerServiceModel customerServiceModel)
		{
			ICustomerService customerService = ServiceHelper.Create<ICustomerService>();
			CustomerServiceInfo customerServiceInfo = new CustomerServiceInfo()
			{
				Id = customerServiceModel.Id,
				Type = customerServiceModel.Type.GetValueOrDefault(CustomerServiceInfo.ServiceType.PreSale),
				Tool = customerServiceModel.Tool,
				Name = customerServiceModel.Name,
				AccountCode = customerServiceModel.Account,
				ShopId = base.CurrentSellerManager.ShopId
			};
			CustomerServiceInfo customerServiceInfo1 = customerServiceInfo;
			if (customerServiceInfo1.Id <= 0)
			{
				customerService.AddCustomerService(customerServiceInfo1);
			}
			else
			{
				customerService.UpdateCustomerService(customerServiceInfo1);
			}
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult Delete(long id)
		{
			ServiceHelper.Create<ICustomerService>().DeleteCustomerService(base.CurrentSellerManager.ShopId, new long[] { id });
			return Json(new { success = true });
		}

		public ActionResult Management()
		{
			CustomerServiceInfo[] array = (
				from item in ServiceHelper.Create<ICustomerService>().GetCustomerService(base.CurrentSellerManager.ShopId)
				orderby item.Id descending
				select item).ToArray();
			IEnumerable<CustomerServiceModel> customerServiceModel = 
				from item in array
                select new CustomerServiceModel()
				{
					Id = item.Id,
					Account = item.AccountCode,
					Name = item.Name,
					Tool = item.Tool,
					Type = new CustomerServiceInfo.ServiceType?(item.Type)
				};
			return View(customerServiceModel);
		}
	}
}