using Himall.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models.Product
{
	public class CategoryModel
	{
		[Range(0.01, 100, ErrorMessage="佣金比不可以超出商品价值")]
		[RegularExpression("^(?!0+(?:\\.0+)?$)\\d+(?:\\.\\d{1,2})?$", ErrorMessage="分佣比例只能是大于等于0的的数字，可保留两位小数")]
		[Required(ErrorMessage="分类佣金比例必填")]
		public decimal CommisRate
		{
			get;
			set;
		}

		public int Depth
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public long DisplaySequence
		{
			get;
			set;
		}

		public bool HasChildren
		{
			get;
			set;
		}

		public string Icon
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public string Keywords
		{
			get;
			set;
		}

		[MaxLength(12, ErrorMessage="分类名称不能多于12个字符")]
		[RegularExpression("^[a-zA-Z0-9_\\u4e00-\\u9fa5]+$", ErrorMessage="分类名称由汉字、字母、数字下划线组成")]
		[Required(ErrorMessage="分类名称必填,且不能多于12个字符")]
		public string Name
		{
			get;
			set;
		}

		public long ParentCategoryId
		{
			get;
			set;
		}

		public string Path
		{
			get;
			set;
		}

		public string RewriteName
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		[RegularExpression("^[0-9]*[1-9][0-9]*$", ErrorMessage="类型必填")]
		[Required(ErrorMessage="类型必填")]
		public long TypeId
		{
			get;
			set;
		}

		public CategoryModel()
		{
		}

		public CategoryModel(CategoryInfo m) : this()
		{
            Id = m.Id;
            Depth = m.Depth;
            DisplaySequence = m.DisplaySequence;
            HasChildren = m.HasChildren;
            Icon = m.Icon;
            Description = m.Meta_Description;
            Keywords = m.Meta_Keywords;
            Title = m.Meta_Title;
            Name = m.Name;
            ParentCategoryId = m.ParentCategoryId;
            RewriteName = m.RewriteName;
            Path = m.Path;
            TypeId = m.TypeId;
            CommisRate = m.CommisRate;
		}

		public static implicit operator CategoryInfo(CategoryModel m)
		{
			CategoryInfo categoryInfo = new CategoryInfo()
			{
				Id = m.Id,
				ParentCategoryId = m.ParentCategoryId,
				Depth = m.Depth,
				DisplaySequence = m.DisplaySequence,
				HasChildren = m.HasChildren,
				Icon = m.Icon,
				Meta_Description = m.Description,
				Meta_Keywords = m.Keywords,
				Meta_Title = m.Title,
				Name = m.Name,
				Path = m.Path,
				RewriteName = m.RewriteName,
				TypeId = m.TypeId,
				CommisRate = m.CommisRate
			};
			return categoryInfo;
		}
	}
}