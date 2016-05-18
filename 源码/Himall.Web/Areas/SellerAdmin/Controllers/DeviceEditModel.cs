using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class DeviceEditModel
	{
		public string apply_reason
		{
			get;
			set;
		}

		public string comment
		{
			get;
			set;
		}

		public long device_id
		{
			get;
			set;
		}

		public int id
		{
			get;
			set;
		}

		public long major
		{
			get;
			set;
		}

		public long minor
		{
			get;
			set;
		}

		public long poi_id
		{
			get;
			set;
		}

		public int quantity
		{
			get;
			set;
		}

		public string uuid
		{
			get;
			set;
		}

		public DeviceEditModel()
		{
		}
	}
}