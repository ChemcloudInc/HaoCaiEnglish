using Senparc.Weixin.MP.AdvancedAPIs.ShakeAround;
using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class DeviceShowModel : DeviceModel
	{
		public string poi_name
		{
			get;
			set;
		}

		public DeviceShowModel(DeviceModel model)
		{
			base.device_id = model.device_id;
			base.uuid = model.uuid;
			base.minor = model.minor;
			base.major = model.major;
			base.comment = model.comment;
			base.page_ids = model.page_ids;
			base.status = model.status;
			base.poi_id = model.poi_id;
		}
	}
}