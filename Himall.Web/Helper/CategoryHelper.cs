using Himall.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Web.Mvc
{
	public static class CategoryHelper
	{
		public static MvcHtmlString GenerateCategorySelect(this HtmlHelper _html, IEnumerable<CategoryInfo> model)
		{
			StringBuilder stringBuilder = new StringBuilder("<option value=\"0\">请选择...</option>");
			foreach (CategoryInfo categoryInfo in model)
			{
				if (categoryInfo.ParentCategoryId != 0)
				{
					continue;
				}
				stringBuilder.Append(string.Concat("<option value=\"", categoryInfo.Id, "\">"));
				stringBuilder.Append(string.Concat(CategoryHelper.GetSpace(categoryInfo.Depth), categoryInfo.Name));
				stringBuilder.Append("</option>");
				stringBuilder.Append(CategoryHelper.GetSubCategory(model, categoryInfo.Id));
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString GenerateSelectHtml(this HtmlHelper _html, List<SelectListItem> model, string name)
		{
			string[] strArrays = new string[] { "<select name=\"", name, "\" id=\"", name, "\" class=\"form-control input-sm\">" };
			StringBuilder stringBuilder = new StringBuilder(string.Concat(strArrays));
			foreach (SelectListItem selectListItem in model)
			{
				string str = (selectListItem.Selected ? "selected" : "");
				string[] value = new string[] { "<option ", str, " value=\"", selectListItem.Value, "\">" };
				stringBuilder.Append(string.Concat(value));
				stringBuilder.Append(selectListItem.Text);
				stringBuilder.Append("</option>");
			}
			stringBuilder.Append("</select>");
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString GenerateTypeSelect(this HtmlHelper _html, IEnumerable<ProductTypeInfo> model, long selectedId = 0L)
		{
			StringBuilder stringBuilder = new StringBuilder("<option value=\"0\">请选择...</option>");
			foreach (ProductTypeInfo productTypeInfo in model)
			{
				string str = (productTypeInfo.Id == selectedId ? "selected" : "");
				object[] id = new object[] { "<option ", str, " value=\"", productTypeInfo.Id, "\">" };
				stringBuilder.Append(string.Concat(id));
				stringBuilder.Append(productTypeInfo.Name);
				stringBuilder.Append("</option>");
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		private static string GetSpace(int depth)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < depth - 1; i++)
			{
				stringBuilder.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
			}
			return stringBuilder.ToString();
		}

		private static string GetSubCategory(IEnumerable<CategoryInfo> list, long Id)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Count(); i++)
			{
				if (list.ElementAt<CategoryInfo>(i).ParentCategoryId == Id)
				{
					stringBuilder.Append(string.Concat("<option value=\"", list.ElementAt<CategoryInfo>(i).Id, "\">"));
					stringBuilder.Append(string.Concat(CategoryHelper.GetSpace(list.ElementAt<CategoryInfo>(i).Depth), list.ElementAt<CategoryInfo>(i).Name));
					stringBuilder.Append("</option>");
				}
			}
			return stringBuilder.ToString();
		}
	}
}