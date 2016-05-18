using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ArticleInfo : BaseModel
	{
		private long _id;

		public DateTime AddDate
		{
			get;
			set;
		}

		public virtual Himall.Model.ArticleCategoryInfo ArticleCategoryInfo
		{
			get;
			set;
		}

		public long CategoryId
		{
			get;
			set;
		}

		public string Content
		{
			get;
			set;
		}

		public long DisplaySequence
		{
			get;
			set;
		}

		public string IconUrl
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

		public bool IsRelease
		{
			get;
			set;
		}

		public string Meta_Description
		{
			get;
			set;
		}

		public string Meta_Keywords
		{
			get;
			set;
		}

		public string Meta_Title
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public ArticleInfo()
		{
		}
	}
}