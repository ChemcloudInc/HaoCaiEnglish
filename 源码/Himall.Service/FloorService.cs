using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
    public class FloorService : ServiceBase, IFloorService, IService, IDisposable
    {
        private static object DisplaySequenceLocker;

        static FloorService()
        {
            FloorService.DisplaySequenceLocker = new object();
        }

        public FloorService()
        {
        }

        public HomeFloorInfo AddHomeFloorBasicInfo(string name, IEnumerable<long> topLevelCategoryIds)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("楼层名称不能为空");
            }
            if (topLevelCategoryIds == null || topLevelCategoryIds.Count() == 0)
            {
                throw new ArgumentNullException("至少要选择一个商品分类");
            }
            HomeFloorInfo homeFloorInfo = new HomeFloorInfo()
            {
                FloorName = name
            };
            lock (FloorService.DisplaySequenceLocker)
            {
                homeFloorInfo.DisplaySequence = GetMaxHomeFloorSequence() + 1;
                foreach (long list in topLevelCategoryIds.ToList())
                {
                    FloorCategoryInfo floorCategoryInfo = new FloorCategoryInfo()
                    {
                        CategoryId = list,
                        Depth = 1
                    };
                    homeFloorInfo.FloorCategoryInfo.Add(floorCategoryInfo);
                }
                context.HomeFloorInfo.Add(homeFloorInfo);
                context.SaveChanges();
            }
            return homeFloorInfo;
        }

        public void DeleteHomeFloor(long id)
        {
            List<long> list = (
                from p in context.FloorTablsInfo
                where p.FloorId == id
                select p.Id).ToList();
            foreach (long num in list)
            {
                context.FloorTablDetailsInfo.RemoveRange(
                    from p in context.FloorTablDetailsInfo
                    where p.TabId == num
                    select p);
            }
            context.FloorTablsInfo.RemoveRange(
                from p in context.FloorTablsInfo
                where p.FloorId == id
                select p);
            context.HomeFloorInfo.Remove(new object[] { id });
            context.SaveChanges();
            RemoveImage(id);
        }

        public void EnableHomeFloor(long homeFloorId, bool enable)
        {
            HomeFloorInfo homeFloorInfo = context.HomeFloorInfo.FindById<HomeFloorInfo>(homeFloorId);
            homeFloorInfo.IsShow = enable;
            context.SaveChanges();
        }

        public HomeFloorInfo GetHomeFloor(long id)
        {
            return context.HomeFloorInfo.FindById<HomeFloorInfo>(id);
        }

        public IQueryable<HomeFloorInfo> GetHomeFloors()
        {
            return
                from a in context.HomeFloorInfo.Include("FloorTopicInfo").Include("FloorBrandInfo").Include("FloorBrandInfo.BrandInfo")
                where a.IsShow
                orderby a.DisplaySequence
                select a;
        }

        private long GetMaxHomeFloorSequence()
        {
            long num = 0;
            if (context.HomeFloorInfo.Count() > 0)
            {
                num = context.HomeFloorInfo.Max<HomeFloorInfo, long>((HomeFloorInfo item) => item.DisplaySequence);
            }
            return num;
        }

        private void RemoveImage(long id, string type)
        {
            string mapPath = IOHelper.GetMapPath(string.Concat("/Storage/Plat/PageSettings/HomeFloor/", id));
            string[] files = Directory.GetFiles(mapPath, string.Concat("floor_", type, "_*"));
            string[] strArrays = files;
            for (int i = 0; i < strArrays.Length; i++)
            {
                string str = strArrays[i];
                try
                {
                    File.Delete(str);
                }
                catch
                {
                }
            }
        }

        private void RemoveImage(long id)
        {
            try
            {
                string mapPath = IOHelper.GetMapPath(string.Concat("/Storage/Plat/PageSettings/HomeFloor/", id));
                Directory.Delete(mapPath, true);
            }
            catch
            {
            }
        }

        private string TransferImage(string sourceFile, long floorId, string type)
        {
            if (string.IsNullOrWhiteSpace(sourceFile) || sourceFile.Contains("/Storage/Plat/"))
            {
                return sourceFile;
            }
            string str = string.Concat("/Storage/Plat/PageSettings/HomeFloor/", floorId);
            string mapPath = IOHelper.GetMapPath(str);
            if (!Directory.Exists(mapPath))
            {
                Directory.CreateDirectory(mapPath);
            }
            string str1 = sourceFile.Substring(sourceFile.LastIndexOf('.'));
            string[] strArrays = new string[] { "floor_", type, "_", null, null };
            strArrays[3] = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            strArrays[4] = str1;
            string str2 = string.Concat(strArrays);
            string mapPath1 = IOHelper.GetMapPath(sourceFile);
            File.Copy(mapPath1, string.Concat(mapPath, "/", str2), true);
            return string.Concat(str, "/", str2);
        }

        private void UpdataProductLink(long floorId, IEnumerable<FloorTopicInfo> productLink)
        {
            context.FloorTopicInfo.OrderBy((FloorTopicInfo item) => item.FloorId == floorId && (int)item.TopicType != 11);
            foreach (FloorTopicInfo floorTopicInfo in productLink)
            {
                floorTopicInfo.TopicImage = TransferImage(floorTopicInfo.TopicImage, floorId, floorTopicInfo.TopicType.ToString());
            }
            context.FloorTopicInfo.Remove(o => o.TopicType != Position.Top);
            context.FloorTopicInfo.AddRange(productLink);
        }

        private void UpdateBrand(long floorId, IEnumerable<FloorBrandInfo> floorBrands)
        {
            IQueryable<FloorBrandInfo> floorBrandInfos = context.FloorBrandInfo.FindBy((FloorBrandInfo item) => item.FloorId == floorId);
            IQueryable<long> brandId =
                from item in floorBrandInfos
                select item.BrandId;
            IEnumerable<long> nums =
                from item in floorBrands
                select item.BrandId;
            IQueryable<FloorBrandInfo> floorBrandInfos1 =
                from item in floorBrandInfos
                where !nums.Contains(item.BrandId)
                select item;
            context.FloorBrandInfo.RemoveRange(floorBrandInfos1);
            IEnumerable<FloorBrandInfo> floorBrandInfos2 =
                from item in floorBrands
                where !brandId.Contains(item.BrandId)
                select item;
            context.FloorBrandInfo.AddRange(floorBrandInfos2);
        }

        private void UpdateCategory(long floorId, IEnumerable<FloorCategoryInfo> floorCategories)
        {
            IQueryable<FloorCategoryInfo> floorCategoryInfos = context.FloorCategoryInfo.FindBy((FloorCategoryInfo item) => item.FloorId == floorId && item.Depth >= 2);
            IQueryable<long> categoryId =
                from item in floorCategoryInfos
                select item.CategoryId;
            IEnumerable<long> nums =
                from item in floorCategories
                select item.CategoryId;
            IQueryable<FloorCategoryInfo> floorCategoryInfos1 =
                from item in floorCategoryInfos
                where !nums.Contains(item.CategoryId)
                select item;
            context.FloorCategoryInfo.RemoveRange(floorCategoryInfos1);
            IEnumerable<FloorCategoryInfo> floorCategoryInfos2 =
                from item in floorCategories
                where !categoryId.Contains(item.CategoryId)
                select item;
            context.FloorCategoryInfo.AddRange(floorCategoryInfos2);
        }

        private void UpdateCategoryImage(long floorId, FloorTopicInfo categoryImage)
        {
            if (categoryImage != null)
            {
                bool flag = false;
                FloorTopicInfo url = context.FloorTopicInfo.FirstOrDefault((FloorTopicInfo item) => item.FloorId == floorId && (int)item.TopicType == 11);
                if (url != null)
                {
                    flag = true;
                }
                else
                {
                    FloorTopicInfo floorTopicInfo = new FloorTopicInfo()
                    {
                        TopicName = "",
                        FloorId = floorId
                    };
                    url = floorTopicInfo;
                }
                url.TopicType = Position.Top;
                url.Url = categoryImage.Url;
                url.TopicImage = TransferImage(categoryImage.TopicImage, floorId, Position.Top.ToString());
                if (!flag)
                {
                    context.FloorTopicInfo.Add(url);
                }
            }
        }

        public void UpdateFloorBasicInfo(long homeFloorId, string name, IEnumerable<long> topLevelCategoryIds)
        {
            IEnumerable<long> nums = topLevelCategoryIds;
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("楼层名称不能为空");
            }
            if (nums == null || nums.Count() == 0)
            {
                throw new ArgumentNullException("至少要选择一个商品分类");
            }
            HomeFloorInfo homeFloorInfo = context.HomeFloorInfo.FindById<HomeFloorInfo>(homeFloorId);
            homeFloorInfo.FloorName = name;
            nums = nums.Distinct<long>();
            IEnumerable<long> floorCategoryInfo =
                from item in homeFloorInfo.FloorCategoryInfo
                select item.CategoryId;
            IEnumerable<long> nums1 =
                from item in floorCategoryInfo
                where !nums.Contains(item)
                select item;
            IEnumerable<long> nums2 =
                from item in nums
                where !floorCategoryInfo.Contains(item)
                select item;
            FloorCategoryInfo[] floorCategoryInfoArray = new FloorCategoryInfo[nums2.Count()];
            int num = 0;
            foreach (long list in nums2.ToList())
            {
                int num1 = num;
                num = num1 + 1;
                FloorCategoryInfo floorCategoryInfo1 = new FloorCategoryInfo()
                {
                    FloorId = homeFloorId,
                    CategoryId = list,
                    Depth = 1
                };
                floorCategoryInfoArray[num1] = floorCategoryInfo1;
            }
            context.FloorCategoryInfo.OrderBy((FloorCategoryInfo item) => item.FloorId == homeFloorId && nums1.Contains(item.CategoryId));
            context.FloorCategoryInfo.AddRange(floorCategoryInfoArray);
            context.SaveChanges();
        }

        public void UpdateHomeFloorDetail(HomeFloorInfo homeFloor)
        {
            if (homeFloor.Id != 0)
            {
                HomeFloorInfo defaultTabName = context.HomeFloorInfo.FindById<HomeFloorInfo>(homeFloor.Id);
                defaultTabName.DefaultTabName = homeFloor.DefaultTabName;
                defaultTabName.FloorName = homeFloor.FloorName;
                defaultTabName.SubName = homeFloor.SubName;
            }
            else
            {
                long num = 0;
                num = (
                    from p in context.HomeFloorInfo
                    select p.DisplaySequence).Max<long>() + 1;
                homeFloor.DisplaySequence = num;
                homeFloor.IsShow = true;
                context.HomeFloorInfo.Add(homeFloor);
            }
            if (homeFloor.StyleLevel == 1)
            {
                UpdateProducts(homeFloor.Id, homeFloor.Himall_FloorTabls);
            }
            UpdateBrand(homeFloor.Id, homeFloor.FloorBrandInfo);
            UpdateTextLink(homeFloor.Id,
                from item in homeFloor.FloorTopicInfo
                where item.TopicType == Position.Top
                select item);
            UpdataProductLink(homeFloor.Id,
                from item in homeFloor.FloorTopicInfo
                where item.TopicType != Position.Top
                select item);
            context.Configuration.ValidateOnSaveEnabled = false;
            context.SaveChanges();
            context.Configuration.ValidateOnSaveEnabled = true;
        }

        public void UpdateHomeFloorSequence(long sourceSequence, long destiSequence)
        {
            HomeFloorInfo homeFloorInfo = context.HomeFloorInfo.FirstOrDefault((HomeFloorInfo item) => item.DisplaySequence == sourceSequence);
            HomeFloorInfo homeFloorInfo1 = context.HomeFloorInfo.FirstOrDefault((HomeFloorInfo item) => item.DisplaySequence == destiSequence);
            homeFloorInfo.DisplaySequence = destiSequence;
            homeFloorInfo1.DisplaySequence = sourceSequence;
            context.SaveChanges();
        }

        private void UpdateProductModule(HomeFloorInfo homeFloor)
        {
        }

        private void UpdateProducts(long floorId, IEnumerable<FloorTablsInfo> tabs)
        {
            IQueryable<long> floorTablsInfo =
                from item in context.FloorTablsInfo
                where item.FloorId == floorId
                select item.Id;
            context.FloorTablDetailsInfo.OrderBy((FloorTablDetailsInfo item) => floorTablsInfo.Contains(item.TabId));
            context.FloorTablsInfo.OrderBy((FloorTablsInfo item) => item.FloorId == floorId);
            context.FloorTablsInfo.Remove(o => true);
            context.FloorTablDetailsInfo.Remove(o => true);
            context.FloorTablsInfo.AddRange(tabs);
            foreach (FloorTablsInfo tab in tabs)
            {
                context.FloorTablDetailsInfo.AddRange(tab.Himall_FloorTablDetails);
            }
        }

        private void UpdateTextLink(long floorId, IEnumerable<FloorTopicInfo> textLinks)
        {
            context.FloorTopicInfo.OrderBy((FloorTopicInfo item) => item.FloorId == floorId && (int)item.TopicType == 11);
            context.FloorTopicInfo.Remove(o => o.TopicType == Position.Top);
            context.FloorTopicInfo.AddRange(textLinks);
        }
    }
}