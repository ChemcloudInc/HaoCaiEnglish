using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class AgreementInfo : BaseModel
	{
		private long _id;

		public string AgreementContent
		{
			get;
			set;
		}

		public int AgreementType
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

		public DateTime LastUpdateTime
		{
			get;
			set;
		}

		public AgreementInfo()
		{
		}

		public enum AgreementTypes
		{
			[Description("买家")]
			Buyers,
			[Description("卖家")]
			Seller
		}
	}
}