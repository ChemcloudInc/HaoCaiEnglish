using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ProductEvaluation : BaseModel
	{
		public DateTime BuyTime
		{
			get;
			set;
		}

		public string Color
		{
			get;
			set;
		}

		public string EvaluationContent
		{
			get;
			set;
		}

		public int EvaluationRank
		{
			get;
			set;
		}

		public bool EvaluationStatus
		{
			get;
			set;
		}

		public DateTime EvaluationTime
		{
			get;
			set;
		}

		public new long Id
		{
			get;
			set;
		}

		public long OrderId
		{
			get;
			set;
		}

		public ProductCommentInfo ProductComment
		{
			get;
			set;
		}

		public long ProductId
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}

		public string ReplyContent
		{
			get;
			set;
		}

		public DateTime ReplyTime
		{
			get;
			set;
		}

		public string Size
		{
			get;
			set;
		}

		public string ThumbnailsUrl
		{
			get;
			set;
		}

		public string Version
		{
			get;
			set;
		}

		public ProductEvaluation()
		{
		}
	}
}