using System;
using System.Runtime.CompilerServices;

namespace Himall.Model.Models
{
	public class OrderCommentInfo
	{
		public long Id
		{
			get;
			set;
		}

		public long OrderId
		{
			get;
			set;
		}

		public string ReviewContent
		{
			get;
			set;
		}

		public DateTime ReviewDate
		{
			get;
			set;
		}

		public OrderCommentInfo()
		{
		}
	}
}