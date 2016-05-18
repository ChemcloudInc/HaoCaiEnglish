using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class WX_MSGGetCouponModel
	{
		public WX_MSGItemBaseModel first
		{
			get;
			set;
		}

		public WX_MSGItemBaseModel keyword1
		{
			get;
			set;
		}

		public WX_MSGItemBaseModel keyword2
		{
			get;
			set;
		}

		public WX_MSGItemBaseModel remark
		{
			get;
			set;
		}

		public WX_MSGGetCouponModel()
		{
            first = new WX_MSGItemBaseModel();
            keyword1 = new WX_MSGItemBaseModel();
            keyword2 = new WX_MSGItemBaseModel();
            remark = new WX_MSGItemBaseModel();
		}
	}
}