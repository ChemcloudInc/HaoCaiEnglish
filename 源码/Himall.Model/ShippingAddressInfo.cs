using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ShippingAddressInfo : BaseModel
	{
		private long _id;

		public string Address
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

		public bool IsDefault
		{
			get;
			set;
		}

		public bool IsQuick
		{
			get;
			set;
		}

		public virtual UserMemberInfo MemberInfo
		{
			get;
			set;
		}

		public string Phone
		{
			get;
			set;
		}

		[NotMapped]
		public string RegionFullName
		{
			get;
			set;
		}

		public int RegionId
		{
			get;
			set;
		}

		[NotMapped]
		public string RegionIdPath
		{
			get;
			set;
		}

		public string ShipTo
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public ShippingAddressInfo()
		{
		}
	}
}