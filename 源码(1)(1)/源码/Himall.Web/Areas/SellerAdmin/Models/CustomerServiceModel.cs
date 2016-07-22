using Himall.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class CustomerServiceModel
	{
		[Required(ErrorMessage="账号必须填写")]
		public string Account
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		[Required(ErrorMessage="客服名称必须填写")]
		public string Name
		{
			get;
			set;
		}

		public CustomerServiceInfo.ServiceTool Tool
		{
			get;
			set;
		}

		public CustomerServiceInfo.ServiceType? Type
		{
			get;
			set;
		}

		public CustomerServiceModel()
		{
		}
	}
}