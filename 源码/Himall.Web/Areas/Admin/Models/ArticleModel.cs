using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models
{
	public class ArticleModel
	{
		public string ArticleCategoryFullPath
		{
			get;
			set;
		}

		[Required(ErrorMessage="文章分类必选")]
		public long? CategoryId
		{
			get;
			set;
		}

		[Required(ErrorMessage="品牌简介必填")]
		public string Content
		{
			get;
			set;
		}

		public string IconUrl
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
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

		[MaxLength(25, ErrorMessage="最多25个字符")]
		[MinLength(3, ErrorMessage="最少3个字符")]
		[Required(ErrorMessage="文章标题必填")]
		public string Title
		{
			get;
			set;
		}

		public ArticleModel()
		{
		}
	}
}