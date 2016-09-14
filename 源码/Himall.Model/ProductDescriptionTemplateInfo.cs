using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ProductDescriptionTemplateInfo : BaseModel
	{
		private long _id;

		public string Content
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

		public string Name
		{
			get;
			set;
		}

		public ProductDescriptionTemplateInfo.TemplatePosition Position
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public ProductDescriptionTemplateInfo()
		{
		}

		public enum TemplatePosition
		{
			[Description("top")]
			Top = 1,
			[Description("bottom")]
			Bottom = 2
		}
	}
}