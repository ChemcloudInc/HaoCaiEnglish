using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Web.Mvc
{
	public static class MVCHelpers
	{
		public static MvcHtmlString CategoryPath(string path, string pName)
		{
			StringBuilder stringBuilder = new StringBuilder("<div class=\"breadcrumb\">");
			try
			{
				pName = (pName.Length > 40 ? string.Concat(pName.Substring(0, 40), " ...") : pName);
				string name = "";
				string str = "";
				string name1 = "";
				string[] strArrays = path.Split(new char[] { '|' });
				if (strArrays.Length > 0)
				{
					name = ServiceHelper.Create<ICategoryService>().GetCategory(long.Parse(strArrays[0])).Name;
				}
				if (strArrays.Length > 1)
				{
					str = ServiceHelper.Create<ICategoryService>().GetCategory(long.Parse(strArrays[1])).Name;
				}
				if (strArrays.Length > 2)
				{
					name1 = ServiceHelper.Create<ICategoryService>().GetCategory(long.Parse(strArrays[2])).Name;
				}
				stringBuilder.AppendFormat("<strong><a href=\"/search?cid={0}\">{1}</a></strong>", long.Parse(strArrays[0]), name);
				stringBuilder.AppendFormat("<span>{0}{1}&nbsp;&gt;&nbsp;<a href=\"\">{2}</a></span>", (string.IsNullOrWhiteSpace(str) ? "" : string.Format("&nbsp;&gt;&nbsp;<a href=\"/search?cid={0}\">{1}</a>", long.Parse(strArrays[1]), str)), (string.IsNullOrWhiteSpace(name1) ? "" : string.Format("&nbsp;&gt;&nbsp;<a href=\"/search?cid={0}\">{1}</a>", long.Parse(strArrays[2]), name1)), pName);
				stringBuilder.AppendFormat("</div>", new object[0]);
			}
			catch
			{
				stringBuilder.AppendFormat("</div>", new object[0]);
			}
			return MvcHtmlString.Create(stringBuilder.ToString());
		}

		public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (pagingInfo.TotalPages != 0)
			{
				TagBuilder tagBuilder = new TagBuilder("a")
				{
					InnerHtml = "上一页"
				};
				if (pagingInfo.CurrentPage == 1)
				{
					tagBuilder.MergeAttribute("class", "prev-disabled");
				}
				else
				{
					tagBuilder.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage - 1));
				}
				stringBuilder.Append(tagBuilder.ToString());
				if (pagingInfo.TotalPages >= 1)
				{
					TagBuilder tagBuilder1 = new TagBuilder("a")
					{
						InnerHtml = "1"
					};
					tagBuilder1.MergeAttribute("href", pageUrl(1));
					if (pagingInfo.CurrentPage == 1)
					{
						tagBuilder1.MergeAttribute("class", "current");
					}
					stringBuilder.Append(tagBuilder1.ToString());
				}
				if (pagingInfo.TotalPages >= 2)
				{
					TagBuilder tagBuilder2 = new TagBuilder("a")
					{
						InnerHtml = "2"
					};
					tagBuilder2.MergeAttribute("href", pageUrl(2));
					if (pagingInfo.CurrentPage == 2)
					{
						tagBuilder2.MergeAttribute("class", "current");
					}
					stringBuilder.Append(tagBuilder2.ToString());
				}
				if (pagingInfo.CurrentPage > 5 && pagingInfo.TotalPages != 6)
				{
					TagBuilder tagBuilder3 = new TagBuilder("span")
					{
						InnerHtml = "..."
					};
					tagBuilder3.MergeAttribute("class", "text");
					stringBuilder.Append(tagBuilder3.ToString());
				}
				if (pagingInfo.CurrentPage > 2)
				{
					int currentPage = pagingInfo.CurrentPage;
				}
				if (pagingInfo.CurrentPage <= 5)
				{
					for (int i = 3; i < 8 && i <= pagingInfo.TotalPages; i++)
					{
						TagBuilder tagBuilder4 = new TagBuilder("a")
						{
							InnerHtml = i.ToString()
						};
						tagBuilder4.MergeAttribute("href", pageUrl(i));
						if (i == pagingInfo.CurrentPage)
						{
							tagBuilder4.MergeAttribute("class", "current");
						}
						stringBuilder.Append(tagBuilder4.ToString());
					}
					if (pagingInfo.TotalPages > 7)
					{
						TagBuilder tagBuilder5 = new TagBuilder("span")
						{
							InnerHtml = "..."
						};
						tagBuilder5.MergeAttribute("class", "text");
						stringBuilder.Append(tagBuilder5.ToString());
					}
				}
				if (pagingInfo.CurrentPage > 5 && pagingInfo.CurrentPage + 5 > pagingInfo.TotalPages)
				{
					int totalPages = pagingInfo.TotalPages - 4;
					if (totalPages == 2)
					{
						totalPages++;
					}
					for (int j = totalPages; j <= pagingInfo.TotalPages; j++)
					{
						TagBuilder tagBuilder6 = new TagBuilder("a")
						{
							InnerHtml = j.ToString()
						};
						tagBuilder6.MergeAttribute("href", pageUrl(j));
						if (j == pagingInfo.CurrentPage)
						{
							tagBuilder6.MergeAttribute("class", "current");
						}
						stringBuilder.Append(tagBuilder6.ToString());
					}
				}
				if (pagingInfo.CurrentPage > 5 && pagingInfo.CurrentPage + 5 <= pagingInfo.TotalPages)
				{
					for (int k = pagingInfo.CurrentPage; k < pagingInfo.CurrentPage + 5; k++)
					{
						TagBuilder tagBuilder7 = new TagBuilder("a")
						{
							InnerHtml = (k - 2).ToString()
						};
						tagBuilder7.MergeAttribute("href", pageUrl(k - 2));
						if (k == pagingInfo.CurrentPage + 2)
						{
							tagBuilder7.MergeAttribute("class", "current");
						}
						stringBuilder.Append(tagBuilder7.ToString());
					}
					TagBuilder tagBuilder8 = new TagBuilder("span")
					{
						InnerHtml = "..."
					};
					tagBuilder8.MergeAttribute("class", "text");
					stringBuilder.Append(tagBuilder8.ToString());
				}
				TagBuilder tagBuilder9 = new TagBuilder("a")
				{
					InnerHtml = "下一页"
				};
				if (pagingInfo.CurrentPage == pagingInfo.TotalPages)
				{
					tagBuilder9.MergeAttribute("class", "next-disabled");
				}
				else
				{
					tagBuilder9.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage + 1));
				}
				stringBuilder.Append(tagBuilder9.ToString());
			}
			return MvcHtmlString.Create(stringBuilder.ToString());
		}

		public static MvcHtmlString PageShow(this HtmlHelper html, PagingInfo pagingInfo)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (pagingInfo.TotalPages != 0)
			{
				object[] currentPage = new object[] { "<font color='#ff6600'><b>", pagingInfo.CurrentPage, "</b></font> / ", pagingInfo.TotalPages, " 页，每页<font color='#ff6600'><b>", pagingInfo.ItemsPerPage, "</b></font> 条，共 <font color='#ff6600'><b>", pagingInfo.TotalItems, "</b></font> 条" };
				stringBuilder.Append(string.Concat(currentPage));
			}
			return MvcHtmlString.Create(stringBuilder.ToString());
		}
	}
}