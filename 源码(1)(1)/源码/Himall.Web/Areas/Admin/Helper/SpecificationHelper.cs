using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Helper
{
	public static class SpecificationHelper
	{
		private static Dictionary<SpecificationType, Func<ProductTypeInfo, MvcHtmlString>> SpecificationMap;

		static SpecificationHelper()
		{
			SpecificationHelper.SpecificationMap = new Dictionary<SpecificationType, Func<ProductTypeInfo, MvcHtmlString>>()
			{
				{ SpecificationType.Color, new Func<ProductTypeInfo, MvcHtmlString>(SpecificationHelper.GenerateColor) },
				{ SpecificationType.Size, new Func<ProductTypeInfo, MvcHtmlString>(SpecificationHelper.GenerateSize) },
				{ SpecificationType.Version, new Func<ProductTypeInfo, MvcHtmlString>(SpecificationHelper.GenerateVersion) }
			};
		}

		private static MvcHtmlString GenerateColor(ProductTypeInfo model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string str = "<label class=\"label-custom\"><input class=\"specCheckbox\" type=\"checkbox\" value=\"{0}\" {1} name=\"IsSupportColor\" />颜色</label>";
			string str1 = "<textarea class=\"form-control input-sm\" {0} name=\"ColorValue\" rows=\"2\">{1}</textarea>";
			if (!model.IsSupportColor)
			{
				stringBuilder.Append(string.Format(str, false, ""));
				stringBuilder.Append(string.Format(str1, "disabled", model.ColorValue));
			}
			else
			{
				stringBuilder.Append(string.Format(str, true, "checked"));
				stringBuilder.Append(string.Format(str1, "", model.ColorValue));
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		private static MvcHtmlString GenerateSize(ProductTypeInfo model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string str = "<label class=\"label-custom\"><input class=\"specCheckbox\" type=\"checkbox\" value=\"{0}\" {1} name=\"IsSupportSize\" />尺寸</label>";
			string str1 = "<textarea class=\"form-control input-sm\" {0} name=\"SizeValue\" rows=\"2\">{1}</textarea>";
			if (!model.IsSupportSize)
			{
				stringBuilder.Append(string.Format(str, false, ""));
				stringBuilder.Append(string.Format(str1, "disabled", model.SizeValue));
			}
			else
			{
				stringBuilder.Append(string.Format(str, true, "checked"));
				stringBuilder.Append(string.Format(str1, "", model.SizeValue));
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString GenerateSpecification(this HtmlHelper _html, ProductTypeInfo model, SpecificationType spec)
		{
			return SpecificationHelper.SpecificationMap[spec](model);
		}

		private static MvcHtmlString GenerateVersion(ProductTypeInfo model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string str = "<label class=\"label-custom\"><input class=\"specCheckbox\" type=\"checkbox\" value=\"{0}\" {1} name=\"IsSupportVersion\" />规格</label>";
			string str1 = "<textarea class=\"form-control input-sm\" {0} name=\"VersionValue\" rows=\"2\">{1}</textarea>";
			if (!model.IsSupportVersion)
			{
				stringBuilder.Append(string.Format(str, false, ""));
				stringBuilder.Append(string.Format(str1, "disabled", model.VersionValue));
			}
			else
			{
				stringBuilder.Append(string.Format(str, true, "checked"));
				stringBuilder.Append(string.Format(str1, "", model.VersionValue));
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}