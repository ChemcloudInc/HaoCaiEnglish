using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.SellerAdmin.Models
{
	public class ShopCategoryModel
	{
		public long Depth
		{
			get;
			set;
		}

		public long DisplaySequence
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
		}

		public bool IsShow
		{
			get;
			set;
		}

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

		public long ShopId
		{
			get;
			set;
		}

		public ShopCategoryModel()
		{
		}

		public ShopCategoryModel(ShopCategoryInfo info) : this()
		{
            Id = info.Id;
            DisplaySequence = info.DisplaySequence;
            IsShow = info.IsShow;
            Name = info.Name;
            ParentCategoryId = info.ParentCategoryId;
            ShopId = info.ShopId;
            Depth = (info.ParentCategoryId != (long)0 ? 2 : 1);
		}

		public static implicit operator ShopCategoryInfo(ShopCategoryModel m)
		{
			ShopCategoryInfo shopCategoryInfo = new ShopCategoryInfo()
			{
				Id = m.Id,
				ParentCategoryId = m.ParentCategoryId,
				IsShow = m.IsShow,
				ShopId = m.ShopId,
				DisplaySequence = m.DisplaySequence,
				Name = m.Name
			};
			return shopCategoryInfo;
		}
	}
}