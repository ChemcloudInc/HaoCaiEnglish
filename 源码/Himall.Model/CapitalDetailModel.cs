using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class CapitalDetailModel
	{
		private string _createTime;

		public decimal Amount
		{
			get;
			set;
		}

		public long CapitalID
		{
			get;
			set;
		}

		public string CreateTime
		{
			get
			{
				return _createTime;
			}
			set
			{
                _createTime = value;
			}
		}

		public long Id
		{
			get;
			set;
		}

		public string PayWay
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public string SourceData
		{
			get;
			set;
		}

		public CapitalDetailInfo.CapitalDetailType SourceType
		{
			get;
			set;
		}

		public long UserId
		{
			get;
			set;
		}

		public CapitalDetailModel()
		{
            _createTime = DateTime.Now.ToString();
		}
	}
}