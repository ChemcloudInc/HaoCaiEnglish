using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class TypeService : ServiceBase, ITypeService, IService, IDisposable
	{
		private Dictionary<SpecificationType, string> _SpecKeyValue = new Dictionary<SpecificationType, string>();

		public TypeService()
		{
		}

		public void AddType(ProductTypeInfo model)
		{
            context.ProductTypeInfo.Add(model);
            context.SaveChanges();
		}

		public void DeleteType(long id)
		{
			ProductTypeInfo productTypeInfo = context.ProductTypeInfo.FindById<ProductTypeInfo>(id);
			if (productTypeInfo.CategoryInfo.Count() > 0)
			{
				throw new HimallException("该类型已经有分类关联，不能删除.");
			}
            context.ProductTypeInfo.Remove(productTypeInfo);
            context.SaveChanges();
		}

		public ProductTypeInfo GetType(long id)
		{
			return context.ProductTypeInfo.FindById<ProductTypeInfo>(id);
		}

		public IQueryable<ProductTypeInfo> GetTypes()
		{
			return context.ProductTypeInfo.FindAll<ProductTypeInfo>();
		}

		public PageModel<ProductTypeInfo> GetTypes(string search, int pageNo, int pageSize)
		{
			bool flag = string.IsNullOrWhiteSpace(search);
			int num = 0;
			IQueryable<ProductTypeInfo> productTypeInfos = context.ProductTypeInfo.FindBy((ProductTypeInfo t) => true);
			IQueryable<ProductTypeInfo> productTypeInfos1 = productTypeInfos.FindBy<ProductTypeInfo, long>((ProductTypeInfo t) => t.Name.Contains(search) || flag, pageNo, pageSize, out num, (ProductTypeInfo t) => t.Id, false);
			productTypeInfos1.ToList();
			return new PageModel<ProductTypeInfo>()
			{
				Total = num,
				Models = productTypeInfos1
			};
		}

		private void ProcessingAttr(ProductTypeInfo model)
		{
            ProcessingAttrDeleteAndAdd(model);
            ProcessingAttrUpdate(model);
		}

		private void ProcessingAttrDeleteAndAdd(ProductTypeInfo model)
		{
			ProductTypeInfo productTypeInfo = context.ProductTypeInfo.FindById<ProductTypeInfo>(model.Id);
			List<AttributeInfo> list = productTypeInfo.AttributeInfo.Except<AttributeInfo>(model.AttributeInfo, new AttrComparer()).ToList();
			foreach (AttributeInfo attributeInfo in list)
			{
				IQueryable<AttributeValueInfo> attributeValueInfos = context.AttributeValueInfo.FindBy((AttributeValueInfo a) => a.AttributeId.Equals(attributeInfo.Id));
				foreach (AttributeValueInfo attributeValueInfo in attributeValueInfos.ToList())
				{
                    context.AttributeValueInfo.Remove(attributeValueInfo);
				}
                context.AttributeInfo.Remove(attributeInfo);
			}
			List<AttributeInfo> attributeInfos = model.AttributeInfo.Except<AttributeInfo>(productTypeInfo.AttributeInfo, new AttrComparer()).ToList();
			if (attributeInfos != null && attributeInfos.Count() > 0)
			{
				foreach (AttributeInfo attributeInfo1 in attributeInfos)
				{
                    context.AttributeInfo.Add(attributeInfo1);
				}
			}
		}

		private void ProcessingAttrUpdate(ProductTypeInfo model)
		{
			ProductTypeInfo productTypeInfo = context.ProductTypeInfo.FindById<ProductTypeInfo>(model.Id);
			foreach (AttributeInfo list in model.AttributeInfo.ToList())
			{
				if (!productTypeInfo.AttributeInfo.Any((AttributeInfo a) => {
					if (!a.Id.Equals(list.Id))
					{
						return false;
					}
					return list.Id != 0;
				}))
				{
					continue;
				}
				AttributeInfo name = productTypeInfo.AttributeInfo.FirstOrDefault((AttributeInfo a) => a.Id.Equals(list.Id));
				name.Name = list.Name;
				name.IsMulti = list.IsMulti;
				IEnumerable<AttributeValueInfo> attributeValueInfos = name.AttributeValueInfo.Except<AttributeValueInfo>(list.AttributeValueInfo, new AttrValueComparer());
				if (attributeValueInfos != null && 0 < attributeValueInfos.Count())
				{
					foreach (AttributeValueInfo attributeValueInfo in attributeValueInfos.ToList())
					{
                        context.AttributeValueInfo.Remove(attributeValueInfo);
					}
				}
				IEnumerable<AttributeValueInfo> attributeValueInfos1 = list.AttributeValueInfo.Except<AttributeValueInfo>(name.AttributeValueInfo, new AttrValueComparer());
				if (attributeValueInfos1 == null || 0 >= attributeValueInfos1.Count())
				{
					continue;
				}
				foreach (AttributeValueInfo id in attributeValueInfos1.ToList())
				{
					id.AttributeId = list.Id;
                    context.AttributeValueInfo.Add(id);
				}
			}
		}

		private void ProcessingBrand(ProductTypeInfo model)
		{
			ProductTypeInfo productTypeInfo = context.ProductTypeInfo.FindById<ProductTypeInfo>(model.Id);
			foreach (TypeBrandInfo list in productTypeInfo.TypeBrandInfo.ToList())
			{
                context.TypeBrandInfo.Remove(list);
			}
			foreach (TypeBrandInfo typeBrandInfo in model.TypeBrandInfo)
			{
                context.TypeBrandInfo.Add(typeBrandInfo);
			}
		}

		private void ProcessingCommon(ProductTypeInfo model)
		{
			ProductTypeInfo name = context.ProductTypeInfo.FindById<ProductTypeInfo>(model.Id);
			name.Name = model.Name;
		}

		private void ProcessingSpecificationValues(ProductTypeInfo model)
		{
			if (model.IsSupportColor)
			{
                UpdateSpecificationValues(model, SpecificationType.Color);
			}
			if (model.IsSupportSize)
			{
                UpdateSpecificationValues(model, SpecificationType.Size);
			}
			if (model.IsSupportVersion)
			{
                UpdateSpecificationValues(model, SpecificationType.Version);
			}
			ProductTypeInfo isSupportVersion = context.ProductTypeInfo.FindById<ProductTypeInfo>(model.Id);
			isSupportVersion.IsSupportVersion = model.IsSupportVersion;
			isSupportVersion.IsSupportColor = model.IsSupportColor;
			isSupportVersion.IsSupportSize = model.IsSupportSize;
		}

		private void UpdateSpecificationValues(ProductTypeInfo model, SpecificationType specEnum)
		{
			IEnumerable<SpecificationValueInfo> specificationValueInfo = 
				from s in model.SpecificationValueInfo
				where s.Specification == specEnum
				select s;
			IEnumerable<SpecificationValueInfo> specificationValueInfos = (
				from s in context.SpecificationValueInfo
				where s.TypeId.Equals(model.Id) && (int)s.Specification == (int)specEnum
				select s).AsEnumerable<SpecificationValueInfo>();
			IEnumerable<SpecificationValueInfo> specificationValueInfos1 = specificationValueInfos.Except<SpecificationValueInfo>(specificationValueInfo, new SpecValueComparer());
			foreach (SpecificationValueInfo list in specificationValueInfos1.ToList())
			{
                context.SpecificationValueInfo.Remove(list);
			}
			IEnumerable<SpecificationValueInfo> specificationValueInfos2 = specificationValueInfo.Except<SpecificationValueInfo>(specificationValueInfos, new SpecValueComparer());
			foreach (SpecificationValueInfo list1 in specificationValueInfos2.ToList())
			{
                context.SpecificationValueInfo.Add(list1);
			}
		}

		public void UpdateType(ProductTypeInfo model)
		{
            ProcessingBrand(model);
            ProcessingAttr(model);
            ProcessingSpecificationValues(model);
            ProcessingCommon(model);
            context.SaveChanges();
		}
	}
}