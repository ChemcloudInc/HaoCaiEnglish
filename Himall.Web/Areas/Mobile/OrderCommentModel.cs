using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Mobile
{
	public class OrderCommentModel
	{
		public int DeliveryMark
		{
			get;
			set;
		}

		public long OrderId
		{
			get;
			set;
		}

		public int PackMark
		{
			get;
			set;
		}

		public IEnumerable<ProductCommentModel> ProductComments
		{
			get;
			set;
		}

		public int Score
		{
			get;
			set;
		}

		public int ServiceMark
		{
			get;
			set;
		}

		public OrderCommentModel()
		{
		}
	}
}