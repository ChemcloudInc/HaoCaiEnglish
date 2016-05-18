using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class OrderOperationLogInfo : BaseModel
	{
		private long _id;

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

		public string OperateContent
		{
			get;
			set;
		}

		public DateTime OperateDate
		{
			get;
			set;
		}

		public string Operator
		{
			get;
			set;
		}

		public long OrderId
		{
			get;
			set;
		}

		public virtual Himall.Model.OrderInfo OrderInfo
		{
			get;
			set;
		}

		public OrderOperationLogInfo()
		{
		}
	}
}