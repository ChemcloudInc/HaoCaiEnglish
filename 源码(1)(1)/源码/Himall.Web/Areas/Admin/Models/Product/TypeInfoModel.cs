using Himall.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Admin.Models.Product
{
	public class TypeInfoModel
	{
		public List<TypeAttribute> Attributes
		{
			get;
			set;
		}

		public List<BrandInfoModel> Brands
		{
			get;
			set;
		}

		public string ColorValue
		{
			get;
			set;
		}

		public long Id
		{
			get;
			set;
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

		[Required(ErrorMessage="类型名称必填")]
		public string Name
		{
			get;
			set;
		}

		public string SizeValue
		{
			get;
			set;
		}

		public string VersionValue
		{
			get;
			set;
		}

		public TypeInfoModel()
		{
		}

		public static implicit operator ProductTypeInfo(TypeInfoModel m)
		{
			ProductTypeInfo productTypeInfo = new ProductTypeInfo()
			{
				Id = m.Id,
				Name = m.Name,
				ColorValue = m.ColorValue,
				SizeValue = m.SizeValue,
				VersionValue = m.VersionValue,
				SpecificationValueInfo = new List<SpecificationValueInfo>(),
				AttributeInfo = new List<AttributeInfo>(),
				TypeBrandInfo = new List<TypeBrandInfo>()
			};
			ProductTypeInfo isSupportColor = productTypeInfo;
			if (m.IsSupportColor && !string.IsNullOrWhiteSpace(m.ColorValue))
			{
				isSupportColor.IsSupportColor = m.IsSupportColor;
				m.ColorValue = m.ColorValue.Replace("，", ",");
				string[] strArrays = m.ColorValue.Split(new char[] { ',' });
				for (int i = 0; i < strArrays.Length; i++)
				{
					string str = strArrays[i];
					if (!string.IsNullOrWhiteSpace(str))
					{
						ICollection<SpecificationValueInfo> specificationValueInfo = isSupportColor.SpecificationValueInfo;
						SpecificationValueInfo specificationValueInfo1 = new SpecificationValueInfo()
						{
							Specification = SpecificationType.Color,
							Value = str,
							TypeId = m.Id
						};
						specificationValueInfo.Add(specificationValueInfo1);
					}
				}
			}
			if (m.IsSupportSize && !string.IsNullOrWhiteSpace(m.SizeValue))
			{
				isSupportColor.IsSupportSize = m.IsSupportSize;
				m.SizeValue = m.SizeValue.Replace("，", ",");
				string[] strArrays1 = m.SizeValue.Split(new char[] { ',' });
				for (int j = 0; j < strArrays1.Length; j++)
				{
					string str1 = strArrays1[j];
					if (!string.IsNullOrWhiteSpace(str1))
					{
						ICollection<SpecificationValueInfo> specificationValueInfos = isSupportColor.SpecificationValueInfo;
						SpecificationValueInfo specificationValueInfo2 = new SpecificationValueInfo()
						{
							Specification = SpecificationType.Size,
							Value = str1,
							TypeId = m.Id
						};
						specificationValueInfos.Add(specificationValueInfo2);
					}
				}
			}
			if (m.IsSupportVersion && !string.IsNullOrWhiteSpace(m.VersionValue))
			{
				isSupportColor.IsSupportVersion = m.IsSupportVersion;
				m.VersionValue = m.VersionValue.Replace("，", ",");
				string[] strArrays2 = m.VersionValue.Split(new char[] { ',' });
				for (int k = 0; k < strArrays2.Length; k++)
				{
					string str2 = strArrays2[k];
					if (!string.IsNullOrWhiteSpace(str2))
					{
						ICollection<SpecificationValueInfo> specificationValueInfos1 = isSupportColor.SpecificationValueInfo;
						SpecificationValueInfo specificationValueInfo3 = new SpecificationValueInfo()
						{
							Specification = SpecificationType.Version,
							Value = str2,
							TypeId = m.Id
						};
						specificationValueInfos1.Add(specificationValueInfo3);
					}
				}
			}
			if (m.Brands != null && m.Brands.Count() > 0)
			{
				foreach (BrandInfoModel brand in m.Brands)
				{
					ICollection<TypeBrandInfo> typeBrandInfo = isSupportColor.TypeBrandInfo;
					TypeBrandInfo typeBrandInfo1 = new TypeBrandInfo()
					{
						BrandId = brand.Id,
						TypeId = m.Id
					};
					typeBrandInfo.Add(typeBrandInfo1);
				}
			}
			if (m.Attributes != null && m.Attributes.Count() > 0)
			{
				foreach (TypeAttribute attribute in m.Attributes)
				{
					AttributeInfo attributeInfo = new AttributeInfo()
					{
						IsMulti = attribute.IsMulti,
						Name = attribute.Name,
						AttributeValueInfo = new List<AttributeValueInfo>(),
						TypeId = m.Id,
						Id = attribute.Id
					};
					AttributeInfo attributeInfo1 = attributeInfo;
					attribute.Value = (string.IsNullOrWhiteSpace(attribute.Value) ? "" : attribute.Value.Replace("，", ","));
					string[] strArrays3 = attribute.Value.Split(new char[] { ',' });
					for (int l = 0; l < strArrays3.Length; l++)
					{
						string str3 = strArrays3[l];
						ICollection<AttributeValueInfo> attributeValueInfo = attributeInfo1.AttributeValueInfo;
						AttributeValueInfo attributeValueInfo1 = new AttributeValueInfo()
						{
							Value = str3,
							DisplaySequence = 1
                        };
						attributeValueInfo.Add(attributeValueInfo1);
					}
					isSupportColor.AttributeInfo.Add(attributeInfo1);
				}
			}
			return isSupportColor;
		}
	}
}