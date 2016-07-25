using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
    public class CollocationService : ServiceBase, ICollocationService, IService, IDisposable
    {
        public CollocationService()
        {
        }

        public void AddCollocation(CollocationInfo info)
        {
            DateTime date = DateTime.Now.Date;
            long num1 = info.Himall_CollocationPoruducts.Where((CollocationPoruductInfo a) =>
            {
                bool flag = true;
                a.IsMain = flag;
                return flag;
            }).Select<CollocationPoruductInfo, long>((CollocationPoruductInfo a) => a.ProductId).FirstOrDefault();
            if (context.CollocationPoruductInfo.Any((CollocationPoruductInfo a) => a.IsMain && a.ProductId == num1 && (a.Himall_Collocation.EndTime > date)))
            {
                throw new HimallException("此主商品已存在组合购，请勿重复添加！");
            }
            context.CollocationInfo.Add(info);
            context.SaveChanges();
        }

        public void CancelCollocation(long CollocationId, long shopId)
        {
            CollocationInfo date = context.CollocationInfo.FirstOrDefault((CollocationInfo a) => a.Id == CollocationId && a.ShopId == shopId);
            if (date != null)
            {
                date.EndTime = DateTime.Now.Date;
            }
            context.SaveChanges();
        }

        public void EditCollocation(CollocationInfo info)
        {
            CollocationInfo title = context.CollocationInfo.FirstOrDefault((CollocationInfo a) => a.Id == info.Id);
            if (title.EndTime < DateTime.Now.Date)
            {
                throw new HimallException("该活动已结束，无法修改！");
            }
            context.CollocationPoruductInfo.RemoveRange(title.Himall_CollocationPoruducts);
            title.Title = info.Title;
            title.ShortDesc = info.ShortDesc;
            title.StartTime = info.StartTime;
            title.EndTime = info.EndTime;
            title.ShopId = info.ShopId;
            title.Himall_CollocationPoruducts = info.Himall_CollocationPoruducts;
            context.SaveChanges();
        }

        public CollocationInfo GetCollocation(long Id)
        {
            return context.CollocationInfo.Include("Himall_CollocationPoruducts").FirstOrDefault((CollocationInfo a) => a.Id == Id);
        }

        public CollocationInfo GetCollocationByProductId(long productId)
        {
            DateTime date = DateTime.Now.Date;
            CollocationPoruductInfo collocationPoruductInfo = context.CollocationPoruductInfo.FirstOrDefault((CollocationPoruductInfo a) => a.ProductId == productId && a.IsMain && (a.Himall_Collocation.StartTime <= date) && (a.Himall_Collocation.EndTime > date));
            if (collocationPoruductInfo == null)
            {
                return null;
            }
            return collocationPoruductInfo.Himall_Collocation;
        }

        public PageModel<CollocationInfo> GetCollocationList(CollocationQuery query)
        {
            int num = 0;
            IQueryable<CollocationModel> collocationInfo =
                from a in context.CollocationInfo
                join b in context.ShopInfo on a.ShopId equals b.Id
                select new CollocationModel()
                {
                    Id = a.Id,
                    CreateTime = a.CreateTime.Value,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Title = a.Title,
                    ShortDesc = a.ShortDesc,
                    ShopName = b.ShopName,
                    ShopId = a.ShopId,
                    ProductId = 0
                };
            collocationInfo =
                from a in collocationInfo
                join b in
                    from t in context.CollocationPoruductInfo
                    where t.IsMain
                    select t on a.Id equals b.ColloId
                select new CollocationModel()
                {
                    Id = a.Id,
                    CreateTime = a.CreateTime.Value,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Title = a.Title,
                    ShortDesc = a.ShortDesc,
                    ShopName = a.ShopName,
                    ShopId = a.ShopId,
                    ProductId = b.ProductId
                };
            if (!string.IsNullOrEmpty(query.Title))
            {
                collocationInfo =
                    from d in collocationInfo
                    where d.Title.Contains(query.Title)
                    select d;
            }
            if (query.ShopId.HasValue)
            {
                collocationInfo =
                    from d in collocationInfo
                    where d.ShopId == query.ShopId.Value
                    select d;
            }
            collocationInfo = collocationInfo.GetPage(out num, query.PageNo, query.PageSize, (IQueryable<CollocationModel> d) =>
                from item in d
                orderby item.CreateTime descending
                select item);
            return new PageModel<CollocationInfo>()
            {
                Models = collocationInfo,
                Total = num
            };
        }

        public CollocationSkuInfo GetColloSku(long colloPid, string skuid)
        {
            return context.CollocationSkuInfo.FirstOrDefault((CollocationSkuInfo a) => a.ColloProductId == colloPid && (a.SkuID == skuid));
        }

        public List<CollocationSkuInfo> GetProductColloSKU(long productid, long colloPid)
        {
            IQueryable<CollocationSkuInfo> collocationSkuInfo =
                from a in context.CollocationSkuInfo
                where a.ColloProductId == colloPid && a.ProductId == productid
                select a;
            return collocationSkuInfo.ToList();
        }
    }
}