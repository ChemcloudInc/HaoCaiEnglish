using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class WeiXinBasicInfo : BaseModel
	{
		private long _id;

		public string AccessToken
		{
			get;
			set;
		}

		public string AppId
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

		public string Ticket
		{
			get;
			set;
		}

		public DateTime TicketOutTime
		{
			get;
			set;
		}

		public WeiXinBasicInfo()
		{
		}
	}
}