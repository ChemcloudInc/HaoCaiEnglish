using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class ProductTypeInfo : BaseModel
	{
		private long _id;

		public virtual ICollection<Himall.Model.AttributeInfo> AttributeInfo
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.CategoryInfo> CategoryInfo
		{
			get;
			set;
		}

		[NotMapped]
		public string ColorValue
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

		public bool IsSupportColor
		{
			get;
			set;
		}

		public bool IsSupportSize
		{
			get;
			set;
		}

		public bool IsSupportVersion
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		[NotMapped]
		public string SizeValue
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.SpecificationValueInfo> SpecificationValueInfo
		{
			get;
			set;
		}

		public virtual ICollection<Himall.Model.TypeBrandInfo> TypeBrandInfo
		{
			get;
			set;
		}

		[NotMapped]
		public string VersionValue
		{
			get;
			set;
		}

		public ProductTypeInfo()
		{
            AttributeInfo = new HashSet<Himall.Model.AttributeInfo>();
            CategoryInfo = new HashSet<Himall.Model.CategoryInfo>();
            SpecificationValueInfo = new HashSet<Himall.Model.SpecificationValueInfo>();
            TypeBrandInfo = new HashSet<Himall.Model.TypeBrandInfo>();
		}

		public ProductTypeInfo(bool initialSpec) : this()
		{
            ColorValue = "紫色,红色,绿色,花色,蓝色,褐色,透明,酒红色,黄色,黑色,深灰色,深紫色,深蓝色";
            SizeValue = "160/80(XS),190/110(XXXL),165/84(S),170/88(M),175/92(L),180/96(XL),185/100(XXL),160/84(XS),165/88(S),170/92(M)";
            VersionValue = "版本1,版本2,版本3,版本4,版本5";
		}
	}
}