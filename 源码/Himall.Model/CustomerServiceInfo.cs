using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CustomerServiceInfo : BaseModel
	{
		private long _id;

		public string AccountCode
		{
			get;
			set;
		}

		public new long Id
		{
			get
			{
				return _id;
			}
			set
			{
                _id = value;
				base.Id = value;
			}
		}

		public string Name
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public CustomerServiceInfo.ServiceTool Tool
		{
			get;
			set;
		}

		public CustomerServiceInfo.ServiceType Type
		{
			get;
			set;
		}

		public CustomerServiceInfo()
		{
		}

		public enum ServiceTool
		{
			[Description("QQ")]
			QQ = 1,
			[Description("旺旺")]
			Wangwang = 2
		}

		public enum ServiceType
		{
			[Description("售前")]
			PreSale = 1,
			[Description("售后")]
			AfterSale = 2
		}
	}
}