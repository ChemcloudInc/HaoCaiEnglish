using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Himall.Web.Models
{
	public class BrandModel
	{
		[MaxLength(200, ErrorMessage="品牌描述不能超过200个字符")]
		public string BrandDesc
		{
			get;
			set;
		}

		public string BrandEnName
		{
			get;
			set;
		}

		[Required(ErrorMessage="品牌图片不能为空！")]
		public string BrandLogo
		{
			get;
			set;
		}

		[MaxLength(20, ErrorMessage="品牌名称不能超过20个字符")]
		[RegularExpression("^[a-zA-Z0-9_一-龥]+$", ErrorMessage="品牌名称必须是中文，字母，数字和下划线")]
		[Required(ErrorMessage="品牌必须填写")]
		public string BrandName
		{
			get;
			set;
		}

		public int DisplaySequence
		{
			get;
			set;
		}

		public long ID
		{
			get;
			set;
		}

		public bool IsRecommend
		{
			get;
			set;
		}

		[MaxLength(500, ErrorMessage="SEO描述不能超过500个字符")]
		public string MetaDescription
		{
			get;
			set;
		}

		[MaxLength(500, ErrorMessage="SEO关键字不能超过500个字符")]
		public string MetaKeyWord
		{
			get;
			set;
		}

		[MaxLength(500, ErrorMessage="SEO标题不能超过500个字符")]
		public string MetaTitle
		{
			get;
			set;
		}

		public BrandModel()
		{
		}
	}
}