using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class ProductDescriptionTemplateService : ServiceBase, IProductDescriptionTemplateService, IService, IDisposable
	{
		private const string TEMPLATE_DIRECTORY = "/Storage/Shop/{0}/templates/{1}";

		public ProductDescriptionTemplateService()
		{
		}

		public void AddTemplate(ProductDescriptionTemplateInfo template)
		{
            CheckProperty(template);
			template.Content = HTMLProcess(template.Content, template.Id, template.ShopId);
            context.ProductDescriptionTemplateInfo.Add(template);
            context.SaveChanges();
		}

		private void CheckProperty(ProductDescriptionTemplateInfo template)
		{
			if (string.IsNullOrWhiteSpace(template.Content))
			{
				throw new InvalidPropertyException("模板内容不可空");
			}
			if (string.IsNullOrWhiteSpace(template.Name))
			{
				throw new InvalidPropertyException("模板名称不可空");
			}
			if (template.ShopId == 0)
			{
				throw new InvalidPropertyException("店铺id不可空");
			}
		}

		public void DeleteTemplate(long shopId, params long[] ids)
		{
			IQueryable<ProductDescriptionTemplateInfo> productDescriptionTemplateInfo = 
				from item in context.ProductDescriptionTemplateInfo
				where ids.Contains(item.Id)
				select item;
			if (productDescriptionTemplateInfo.Count((ProductDescriptionTemplateInfo item) => item.ShopId != shopId) > 0)
			{
				throw new HimallException("不能删除非本店铺的商品描述模板");
			}
			IEnumerable<string> array = (
				from item in productDescriptionTemplateInfo
				select item.Id.ToString()).ToArray();
			array = 
				from item in array
				select string.Format("/Storage/Shop/{0}/templates/{1}", shopId, item);
			foreach (string str in array)
			{
				Directory.Delete(IOHelper.GetMapPath(str), true);
			}
            context.ProductDescriptionTemplateInfo.OrderBy((ProductDescriptionTemplateInfo item) => ids.Contains(item.Id));
            context.SaveChanges();
		}

		public ProductDescriptionTemplateInfo GetTemplate(long id, long shopId)
		{
			return context.ProductDescriptionTemplateInfo.FindBy((ProductDescriptionTemplateInfo item) => item.Id == id && item.ShopId == shopId).FirstOrDefault();
		}

		public PageModel<ProductDescriptionTemplateInfo> GetTemplates(long shopId, int pageNumber, int pageSize, string name = null, ProductDescriptionTemplateInfo.TemplatePosition? position = null)
		{
			IQueryable<ProductDescriptionTemplateInfo> productDescriptionTemplateInfos = context.ProductDescriptionTemplateInfo.FindBy((ProductDescriptionTemplateInfo item) => item.ShopId == shopId);
			if (position.HasValue)
			{
				productDescriptionTemplateInfos = 
					from item in productDescriptionTemplateInfos
					where (int)item.Position == (int)position.Value
					select item;
			}
			if (!string.IsNullOrWhiteSpace(name))
			{
				productDescriptionTemplateInfos = 
					from item in productDescriptionTemplateInfos
					where item.Name.Contains(name)
					select item;
			}
			PageModel<ProductDescriptionTemplateInfo> pageModel = new PageModel<ProductDescriptionTemplateInfo>()
			{
				Total = productDescriptionTemplateInfos.Count(),
				Models = (
					from item in productDescriptionTemplateInfos
					orderby item.Id descending
					select item).Skip(pageSize * (pageNumber - 1)).Take(pageSize)
			};
			return pageModel;
		}

		public IQueryable<ProductDescriptionTemplateInfo> GetTemplates(long shopId)
		{
			return context.ProductDescriptionTemplateInfo.FindBy((ProductDescriptionTemplateInfo t) => t.ShopId.Equals(shopId));
		}

		private string HTMLProcess(string content, long id, long shopId)
		{
			string str = string.Format("/Storage/Shop/{0}/templates/{1}", shopId, id);
			string mapPath = IOHelper.GetMapPath(str);
			content = HtmlContentHelper.TransferToLocalImage(content, IOHelper.GetMapPath("/"), mapPath, string.Concat(str, "/"));
			content = HtmlContentHelper.RemoveScriptsAndStyles(content);
			return content;
		}

		public void UpdateTemplate(ProductDescriptionTemplateInfo template)
		{
            CheckProperty(template);
			ProductDescriptionTemplateInfo name = context.ProductDescriptionTemplateInfo.FindById<ProductDescriptionTemplateInfo>(template.Id);
			name.Name = template.Name;
			name.Position = template.Position;
			name.Content = HTMLProcess(template.Content, template.Id, template.ShopId);
            context.SaveChanges();
		}
	}
}