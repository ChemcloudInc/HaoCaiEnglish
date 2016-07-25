using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class LogInfo : BaseModel
	{
		private long _id;

		public DateTime Date
		{
			get;
			set;
		}

		public string Description
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

		public string IPAddress
		{
			get;
			set;
		}

		public string PageUrl
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public LogInfo()
		{
		}
	}
}