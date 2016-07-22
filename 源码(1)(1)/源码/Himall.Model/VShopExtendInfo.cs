using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class VShopExtendInfo : BaseModel
	{
		private long _id;

		public DateTime AddTime
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

		public int? Sequence
		{
			get;
			set;
		}

		public int State
		{
			get;
			set;
		}

		public VShopExtendInfo.VShopExtendType Type
		{
			get;
			set;
		}

		public long VShopId
		{
			get;
			set;
		}

		public virtual Himall.Model.VShopInfo VShopInfo
		{
			get;
			set;
		}

		public VShopExtendInfo()
		{
		}

		public enum VShopExtendState
		{
			[Description("未审核")]
			NotAudit = 1,
			[Description("审核通过")]
			Through = 2,
			[Description("审核拒绝")]
			Refused = 3,
			[Description("微店已关闭")]
			Close = 4
		}

		public enum VShopExtendType
		{
			[Description("主推微店")]
			TopShow = 1,
			[Description("热门微店")]
			HotVShop = 2
		}
	}
}