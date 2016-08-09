using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Areas.Admin.Models.Product;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class PageSettingsController : BaseAdminController
	{
		public PageSettingsController()
		{
		}

		public ActionResult AddHomeFloor(long id = 0L)
		{
			HomeFloorInfo homeFloor = ServiceHelper.Create<IFloorService>().GetHomeFloor(id) ?? new HomeFloorInfo();
			ViewBag.TopLevelCategories = ServiceHelper.Create<ICategoryService>().GetCategoryByParentId(0);
			return View(homeFloor);
		}

		public ActionResult AddHomeFloorDetail(long id = 0L)
		{
			string str;
			string str1;
			HomeFloorDetail homeFloorDetail = new HomeFloorDetail()
			{
				Id = 0
            };
			HomeFloorInfo homeFloor = ServiceHelper.Create<IFloorService>().GetHomeFloor(id);
			if (homeFloor != null)
			{
				homeFloorDetail.Id = homeFloor.Id;
				homeFloorDetail.TextLinks = 
					from item in homeFloor.FloorTopicInfo
					where item.TopicType == Position.Top
					select new HomeFloorDetail.TextLink()
					{
						Id = item.Id,
						Name = item.TopicName,
						Url = item.Url
					};
				homeFloorDetail.ProductLinks = 
					from item in homeFloor.FloorTopicInfo
					where item.TopicType != Position.Top
					select new HomeFloorDetail.TextLink()
					{
						Id = (long)item.TopicType,
						Name = item.Url,
						Url = item.TopicImage
					} into i
					orderby i.Id
					select i;
			}
			else
			{
				homeFloor = new HomeFloorInfo();
				List<HomeFloorDetail.TextLink> textLinks = new List<HomeFloorDetail.TextLink>();
				for (int num = 0; num < 10; num++)
				{
					HomeFloorDetail.TextLink textLink = new HomeFloorDetail.TextLink()
					{
						Id = num,
						Name = "",
						Url = ""
					};
					textLinks.Add(textLink);
				}
				homeFloorDetail.ProductLinks = textLinks;
			}
			dynamic viewBag = base.ViewBag;
			str = (homeFloor == null ? "" : homeFloor.FloorName);
			viewBag.FloorName = str;
			dynamic obj = base.ViewBag;
			str1 = (homeFloor == null ? "" : homeFloor.SubName);
			obj.SubName = str1;
			return View(homeFloorDetail);
		}

		public ActionResult AddHomeFloorDetail2(long id = 0L)
		{
			string str;
			string str1;
			HomeFloorDetail homeFloorDetail = new HomeFloorDetail()
			{
				Id = 0
            };
			HomeFloorInfo homeFloor = ServiceHelper.Create<IFloorService>().GetHomeFloor(id);
			if (homeFloor != null)
			{
				homeFloorDetail.Id = homeFloor.Id;
				homeFloorDetail.DefaultTabName = homeFloor.DefaultTabName;
				homeFloorDetail.TextLinks = 
					from item in homeFloor.FloorTopicInfo
					where item.TopicType == Position.Top
					select new HomeFloorDetail.TextLink()
					{
						Id = item.Id,
						Name = item.TopicName,
						Url = item.Url
					};
				homeFloorDetail.ProductLinks = 
					from item in homeFloor.FloorTopicInfo
					where item.TopicType != Position.Top
					select new HomeFloorDetail.TextLink()
					{
						Id = (long)item.TopicType,
						Name = item.Url,
						Url = item.TopicImage
					} into i
					orderby i.Id
					select i;
				homeFloorDetail.Tabs = 
					from item in homeFloor.Himall_FloorTabls
					where item.FloorId == homeFloor.Id
					select item into p
					orderby p.Id
					select p into item
					select new HomeFloorDetail.Tab()
					{
						Id = item.Id,
						Detail = 
							from detail in item.Himall_FloorTablDetails
							where detail.TabId == item.Id
							select detail into p
							select new HomeFloorDetail.ProductDetail()
							{
								Id = p.Id,
								ProductId = p.ProductId
							},
						Name = item.Name,
						Count = (
							from detail in item.Himall_FloorTablDetails
							where detail.TabId == item.Id
							select detail).Count(),
						Ids = ArrayToString((
							from detail in item.Himall_FloorTablDetails
							where detail.TabId == item.Id
							select detail into p
							select p.ProductId).ToArray())
					};
			}
			else
			{
				homeFloor = new HomeFloorInfo();
				List<HomeFloorDetail.TextLink> textLinks = new List<HomeFloorDetail.TextLink>();
				for (int num = 0; num < 12; num++)
				{
					HomeFloorDetail.TextLink textLink = new HomeFloorDetail.TextLink()
					{
						Id = num,
						Name = "",
						Url = ""
					};
					textLinks.Add(textLink);
				}
				homeFloorDetail.ProductLinks = textLinks;
				homeFloorDetail.StyleLevel = 1;
			}
			if (homeFloorDetail.Tabs == null)
			{
				homeFloorDetail.Tabs = new List<HomeFloorDetail.Tab>();
			}
			dynamic viewBag = base.ViewBag;
			str = (homeFloor == null ? "" : homeFloor.FloorName);
			viewBag.FloorName = str;
			dynamic obj = base.ViewBag;
			str1 = (homeFloor == null ? "" : homeFloor.SubName);
			obj.SubName = str1;
			return View(homeFloorDetail);
		}

		private string ArrayToString(long[] array)
		{
			string empty = string.Empty;
			long[] numArray = array;
            if (numArray.Length > 0)
            {
                for (int i = 0; i < numArray.Length; i++)
                {
                    long num = numArray[i];
                    empty = string.Concat(empty, num, ",");
                }
                return empty.Substring(0, empty.Length - 1);
            }
            else
            {
                return empty;
            }
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult ChangeSequence(int oriRowNumber, int newRowNumber)
		{
			ServiceHelper.Create<IHomeCategoryService>().UpdateHomeCategorySetSequence(oriRowNumber, newRowNumber);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult DeleteFloor(long id)
		{
			ServiceHelper.Create<IFloorService>().DeleteHomeFloor(id);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult FloorChangeSequence(int oriRowNumber, int newRowNumber)
		{
			ServiceHelper.Create<IFloorService>().UpdateHomeFloorSequence(oriRowNumber, newRowNumber);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult FloorEnableDisplay(long id, bool enable)
		{
			ServiceHelper.Create<IFloorService>().EnableHomeFloor(id, enable);
			return Json(new { success = true });
		}

		[UnAuthorize]
		public JsonResult GetBrandsAjax(long id = 0L)
		{
			IEnumerable<HomeFloorDetail.Brand> floorBrandInfo = null;
			IQueryable<BrandInfo> brands = ServiceHelper.Create<IBrandService>().GetBrands("");
			if (id != 0)
			{
				floorBrandInfo = 
					from item in ServiceHelper.Create<IFloorService>().GetHomeFloor(id).FloorBrandInfo
					select new HomeFloorDetail.Brand()
					{
						Id = item.BrandId,
						Name = item.BrandInfo.Name
					};
			}
			if (id != 0)
			{
				ServiceHelper.Create<ITypeService>().GetType(id);
			}
			List<BrandViewModel> brandViewModels = new List<BrandViewModel>();
			foreach (BrandInfo brand in brands)
			{
				List<BrandViewModel> brandViewModels1 = brandViewModels;
				BrandViewModel brandViewModel = new BrandViewModel()
				{
					id = brand.Id,
					isChecked = (floorBrandInfo == null ? false : floorBrandInfo.Any<HomeFloorDetail.Brand>((HomeFloorDetail.Brand b) => b.Id.Equals(brand.Id))),
					@value = brand.Name
				};
				brandViewModels1.Add(brandViewModel);
			}
			return Json(new { data = brandViewModels }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult GetHomeCategoryTopics(int rowNumber)
		{
			HomeCategorySet.HomeCategoryTopic homeCategoryTopic;
			HomeCategorySet.HomeCategoryTopic homeCategoryTopic1;
			HomeCategorySet homeCategorySet = ServiceHelper.Create<IHomeCategoryService>().GetHomeCategorySet(rowNumber);
			int num = (homeCategorySet.HomeCategoryTopics == null ? 0 : homeCategorySet.HomeCategoryTopics.Count<HomeCategorySet.HomeCategoryTopic>());
			if (num > 0)
			{
				homeCategoryTopic = homeCategorySet.HomeCategoryTopics.ElementAt<HomeCategorySet.HomeCategoryTopic>(0);
			}
			else
			{
				homeCategoryTopic = null;
			}
			HomeCategorySet.HomeCategoryTopic homeCategoryTopic2 = homeCategoryTopic;
			if (num > 1)
			{
				homeCategoryTopic1 = homeCategorySet.HomeCategoryTopics.ElementAt<HomeCategorySet.HomeCategoryTopic>(1);
			}
			else
			{
				homeCategoryTopic1 = null;
			}
			HomeCategorySet.HomeCategoryTopic homeCategoryTopic3 = homeCategoryTopic1;
			return Json(new { imageUrl1 = (homeCategoryTopic2 == null ? "" : homeCategoryTopic2.ImageUrl), url1 = (homeCategoryTopic2 == null ? "" : homeCategoryTopic2.Url), imageUrl2 = (homeCategoryTopic3 == null ? "" : homeCategoryTopic3.ImageUrl), url2 = (homeCategoryTopic3 == null ? "" : homeCategoryTopic3.Url) });
		}

		private IEnumerable<string> GetTopLevelCategoryNames(IEnumerable<long> categoryIds)
		{
			return 
				from item in ServiceHelper.Create<ICategoryService>().GetTopLevelCategories(categoryIds)
				select item.Name;
		}

		public ActionResult HomeCategory()
		{
			HomeCategorySet[] array = ServiceHelper.Create<IHomeCategoryService>().GetHomeCategorySets().ToArray();
			IOrderedEnumerable<HomeCategory> homeCategory = 
				from item in array
				select new HomeCategory()
				{
					RowNumber = item.RowNumber,
					TopCategoryNames = GetTopLevelCategoryNames(
						from category in item.HomeCategories
						select category.CategoryId),
					AllCategoryIds = 
						from category in item.HomeCategories
						select category.CategoryId
				} into item
				orderby item.RowNumber
				select item;
			return View(homeCategory);
		}

		public ActionResult HomeFloor()
		{
			IQueryable<HomeFloorInfo> homeFloors = ServiceHelper.Create<IFloorService>().GetHomeFloors();
			homeFloors.ToList();
			IQueryable<HomeFloor> homeFloor = 
				from item in homeFloors
				select new HomeFloor()
				{
					DisplaySequence = item.DisplaySequence,
					Enable = item.IsShow,
					Id = item.Id,
					Name = item.FloorName,
					StyleLevel = item.StyleLevel
				};
			return View(homeFloor);
		}

		private string HTMLProcess(string content)
		{
			string str = "/Storage/Plat/PageSettings/PageFoot";
			string mapPath = IOHelper.GetMapPath(str);
			content = HtmlContentHelper.TransferToLocalImage(content, IOHelper.GetMapPath("/"), mapPath, string.Concat(str, "/"));
			content = HtmlContentHelper.RemoveScriptsAndStyles(content);
			return content;
		}

		public ActionResult Index()
		{
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			ViewBag.Logo = siteSettings.Logo;
			ViewBag.Keyword = siteSettings.Keyword;
			ViewBag.Hotkeywords = siteSettings.Hotkeywords;
			dynamic viewBag = base.ViewBag;
			IEnumerable<ImageAdInfo> imageAds = ServiceHelper.Create<ISlideAdsService>().GetImageAds(0);
			viewBag.ImageAdsTop = imageAds.Where((ImageAdInfo p) => {
				if (p.Id == 14)
				{
					return true;
				}
				return p.Id == 15;
			}).ToList();
			ImageAdInfo[] array = ServiceHelper.Create<ISlideAdsService>().GetImageAds(0).Where((ImageAdInfo p) => {
				if (p.Id <= 0)
				{
					return false;
				}
				return p.Id <= 8;
			}).ToArray();
			return View(array);
		}

		private string MoveImages(string image)
		{
			string mapPath = IOHelper.GetMapPath(image);
			string extension = (new FileInfo(mapPath)).Extension;
			string empty = string.Empty;
			empty = IOHelper.GetMapPath("/Storage/Plat/Site");
			string str = "/Storage/Plat/Site/";
			if (!Directory.Exists(empty))
			{
				Directory.CreateDirectory(empty);
			}
			if (image.Replace("\\", "/").Contains("/temp/"))
			{
				IOHelper.CopyFile(mapPath, empty, true, string.Concat("logo", extension));
			}
			return string.Concat(str, "logo", extension);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SaveHomeCategory(string categoryIds, int rowNumber)
		{
			IEnumerable<long> nums;
			if (!string.IsNullOrWhiteSpace(categoryIds))
			{
				char[] chrArray = new char[] { ',' };
				nums = 
					from item in categoryIds.Split(chrArray)
					select long.Parse(item);
			}
			else
			{
				nums = new List<long>();
			}
			ServiceHelper.Create<IHomeCategoryService>().UpdateHomeCategorySet(rowNumber, nums);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SaveHomeFloorBasicInfo(long id, string name, string categoryIds)
		{
			string empty = string.Empty;
			bool flag = false;
			if (string.IsNullOrWhiteSpace(name))
			{
				empty = "楼层名称不能为空";
			}
			else if (name.Trim().Length <= 4)
			{
				name = name.Trim();
				try
				{
					char[] chrArray = new char[] { ',' };
					IEnumerable<long> nums = 
						from item in categoryIds.Split(chrArray)
						where !string.IsNullOrWhiteSpace(item)
						select long.Parse(item);
					if (id <= 0)
					{
						HomeFloorInfo homeFloorInfo = ServiceHelper.Create<IFloorService>().AddHomeFloorBasicInfo(name, nums);
						id = homeFloorInfo.Id;
					}
					else
					{
						ServiceHelper.Create<IFloorService>().UpdateFloorBasicInfo(id, name, nums);
					}
					flag = true;
				}
				catch (FormatException formatException)
				{
					empty = "商品分类编号有误";
				}
			}
			else
			{
				empty = "楼层名称长度不能超过4个字";
			}
			return Json(new { success = flag, msg = empty, id = id });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SaveHomeFloorDetail(string floorDetail)
		{
			HomeFloorDetail homeFloorDetail = JsonConvert.DeserializeObject<HomeFloorDetail>(floorDetail);
			HomeFloorInfo homeFloorInfo = new HomeFloorInfo()
			{
				Id = homeFloorDetail.Id,
				FloorName = homeFloorDetail.Name,
				SubName = homeFloorDetail.SubName,
				DefaultTabName = homeFloorDetail.DefaultTabName,
				StyleLevel = homeFloorDetail.StyleLevel,
				FloorBrandInfo = (
					from item in homeFloorDetail.Brands
					select new FloorBrandInfo()
					{
						BrandId = item.Id,
						FloorId = homeFloorDetail.Id
					}).ToList(),
				FloorTopicInfo = (
					from i in homeFloorDetail.TextLinks
					select new FloorTopicInfo()
					{
						FloorId = homeFloorDetail.Id,
						TopicImage = "",
						TopicName = i.Name,
						Url = i.Url,
						TopicType = Position.Top
					}).ToList()
			};
			HomeFloorInfo list = homeFloorInfo;
			List<FloorTopicInfo> floorTopicInfos = (
				from i in homeFloorDetail.ProductLinks
				select new FloorTopicInfo()
				{
					FloorId = homeFloorDetail.Id,
					TopicImage = (string.IsNullOrWhiteSpace(i.Name) ? "" : i.Name),
					TopicName = "",
					Url = i.Url,
					TopicType = (Position)((int)i.Id)
				}).ToList();
			foreach (FloorTopicInfo floorTopicInfo in floorTopicInfos)
			{
				list.FloorTopicInfo.Add(floorTopicInfo);
			}
			if (homeFloorDetail.Tabs != null)
			{
				list.Himall_FloorTabls = (
					from item in homeFloorDetail.Tabs
					select new FloorTablsInfo()
					{
						Id = item.Id,
						Name = item.Name,
						FloorId = homeFloorDetail.Id,
						Himall_FloorTablDetails = (
							from d in item.Detail
							select new FloorTablDetailsInfo()
							{
								Id = d.Id,
								TabId = item.Id,
								ProductId = d.ProductId
							}).ToList()
					}).ToList();
			}
			ServiceHelper.Create<IFloorService>().UpdateHomeFloorDetail(list);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SaveHomeTopic(int rowNumber, string url1, string imgUrl1, string url2, string imgUrl2)
		{
			HomeCategorySet.HomeCategoryTopic[] homeCategoryTopicArray = new HomeCategorySet.HomeCategoryTopic[2];
			HomeCategorySet.HomeCategoryTopic homeCategoryTopic = new HomeCategorySet.HomeCategoryTopic()
			{
				Url = url1,
				ImageUrl = imgUrl1
			};
			homeCategoryTopicArray[0] = homeCategoryTopic;
			HomeCategorySet.HomeCategoryTopic homeCategoryTopic1 = new HomeCategorySet.HomeCategoryTopic()
			{
				ImageUrl = imgUrl2,
				Url = url2
			};
			homeCategoryTopicArray[1] = homeCategoryTopic1;
			ServiceHelper.Create<IHomeCategoryService>().UpdateHomeCategorySet(rowNumber, homeCategoryTopicArray);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SetKeyWords(string keyword, string hotkeywords)
		{
			ISiteSettingService siteSettingService = ServiceHelper.Create<ISiteSettingService>();
			SiteSettingsInfo siteSettings = siteSettingService.GetSiteSettings();
			siteSettings.Hotkeywords = hotkeywords;
			siteSettings.Keyword = keyword;
			siteSettingService.SetSiteSettings(siteSettings);
			return Json(new Result()
			{
				success = true
			});
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SetLogo(string logo)
		{
			string str = MoveImages(logo);
			ServiceHelper.Create<ISiteSettingService>().SaveSetting("Logo", str);
			return Json(new { success = true, logo = str });
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult SetPageFoot(string content)
		{
			content = HTMLProcess(content);
			ServiceHelper.Create<ISiteSettingService>().SaveSetting("PageFoot", content);
			return Json(new { success = true });
		}

		public ActionResult SetPageFoot()
		{
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			ViewBag.PageFoot = siteSettings.PageFoot;
			return View();
		}
	}
}