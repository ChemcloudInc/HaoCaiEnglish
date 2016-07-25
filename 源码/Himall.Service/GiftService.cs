using Himall.Core;
using Himall.Core.Helper;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
    public class GiftService : ServiceBase, IGiftService, IService, IDisposable
    {
        public GiftService()
        {
        }

        public void AddGift(GiftInfo model)
        {
            context.GiftInfo.Add(model);
            context.SaveChanges();
            if (string.IsNullOrWhiteSpace(model.ImagePath))
            {
                model.ImagePath = string.Format("/Storage/Gift/{0}", model.Id);
            }
            context.SaveChanges();
            model.Description = HTMLProcess(model.Description, model.ImagePath);
            context.SaveChanges();
        }

        public void ChangeStatus(long id, bool status)
        {
            GiftInfo giftInfo = context.GiftInfo.FindById<GiftInfo>(id);
            if (giftInfo == null)
            {
                throw new HimallException(string.Concat("未找到id为", id, "的礼品"));
            }
            if (!status)
            {
                giftInfo.SalesStatus = GiftInfo.GiftSalesStatus.OffShelves;
            }
            else
            {
                giftInfo.SalesStatus = GiftInfo.GiftSalesStatus.Normal;
                if (giftInfo.EndDate.Date < DateTime.Now.Date)
                {
                    giftInfo.EndDate = DateTime.Now.AddMonths(1);
                }
            }
            context.SaveChanges();
        }

        public GiftInfo GetById(long id)
        {
            GiftInfo integral = null;
            integral = context.GiftInfo.FirstOrDefault((GiftInfo d) => d.Id == id);
            if (integral != null)
            {
                if (integral.NeedGrade == 0)
                {
                    integral.GradeIntegral = 0;
                    integral.NeedGradeName = "不限等级";
                }
                else
                {
                    MemberGrade memberGrade = context.MemberGrade.FirstOrDefault((MemberGrade d) => d.Id == integral.NeedGrade);
                    if (memberGrade == null)
                    {
                        integral.GradeIntegral = -1;
                        integral.NeedGradeName = "等级己删除";
                    }
                    else
                    {
                        integral.GradeIntegral = memberGrade.Integral;
                        integral.NeedGradeName = memberGrade.GradeName;
                    }
                }
            }
            return integral;
        }

        public GiftInfo GetByIdAsNoTracking(long id)
        {
            return context.GiftInfo.AsNoTracking().FirstOrDefault((GiftInfo d) => d.Id == id);
        }

        public PageModel<GiftModel> GetGifts(GiftQuery query)
        {
            PageModel<GiftModel> pageModel = new PageModel<GiftModel>()
            {
                Total = 0
            };
            IQueryable<GiftModel> salesStatus = (
                from d in context.GiftInfo
                join b in context.MemberGrade on d.NeedGrade equals b.Id into j1
                from jd in j1.DefaultIfEmpty<MemberGrade>()
                select new GiftModel()
                {
                    Id = d.Id,
                    GiftName = d.GiftName,
                    NeedIntegral = d.NeedIntegral,
                    LimtQuantity = d.LimtQuantity,
                    StockQuantity = d.StockQuantity,
                    EndDate = d.EndDate,
                    NeedGrade = d.NeedGrade,
                    VirtualSales = d.VirtualSales,
                    RealSales = d.RealSales,
                    SalesStatus = d.SalesStatus,
                    ImagePath = d.ImagePath,
                    Sequence = d.Sequence,
                    GiftValue = d.GiftValue,
                    AddDate = d.AddDate,
                    GradeIntegral = (jd == null ? 0 : jd.Integral),
                    NeedGradeName = (jd == null ? "不限等级" : jd.GradeName)
                }).AsQueryable<GiftModel>();
            if (!string.IsNullOrWhiteSpace(query.skey))
            {
                salesStatus =
                    from d in salesStatus
                    where d.GiftName.Contains(query.skey)
                    select d;
            }
            if (query.status.HasValue)
            {
                DateTime now = DateTime.Now;
                DateTime date = now.AddDays(1).Date;
                GiftInfo.GiftSalesStatus? nullable = query.status;
                GiftInfo.GiftSalesStatus valueOrDefault = nullable.GetValueOrDefault();
                if (nullable.HasValue)
                {
                    switch (valueOrDefault)
                    {
                        case GiftInfo.GiftSalesStatus.Normal:
                            {
                                salesStatus =
                                    from d in salesStatus
                                    where (int)d.SalesStatus == 1 && (d.EndDate >= now)
                                    select d;

                            }
                            break;
                        case GiftInfo.GiftSalesStatus.HasExpired:
                            {
                                salesStatus =
                                    from d in salesStatus
                                    where (int)d.SalesStatus == 1 && (d.EndDate < now)
                                    select d;
                            }
                            break;
                    }
                }
                else
                {
                    salesStatus =
                        from d in salesStatus
                        where (int?)d.SalesStatus == (int?)query.status
                        select d;
                }
            }
            bool? nullable1 = query.isShowAll;
            if ((!nullable1.GetValueOrDefault() ? true : !nullable1.HasValue))
            {
                salesStatus =
                    from d in salesStatus
                    where (int)d.SalesStatus != -1
                    select d;
            }
            Func<IQueryable<GiftModel>, IOrderedQueryable<GiftModel>> orderBy = salesStatus.GetOrderBy<GiftModel>((IQueryable<GiftModel> d) =>
                from o in d
                orderby o.Sequence, o.Id descending
                select o);
            switch (query.Sort)
            {
                case GiftQuery.GiftSortEnum.SalesNumber:
                    {
                        if (!query.IsAsc)
                        {
                            orderBy = salesStatus.GetOrderBy<GiftModel>((IQueryable<GiftModel> o) =>
                                from d in o
                                orderby d.RealSales descending, d.Id descending
                                select d);
                            break;
                        }
                        else
                        {
                            orderBy = salesStatus.GetOrderBy<GiftModel>((IQueryable<GiftModel> o) =>
                                from d in o
                                orderby d.RealSales, d.Id descending
                                select d);
                            break;
                        }
                    }
                case GiftQuery.GiftSortEnum.RealSalesNumber:
                    {
                        if (!query.IsAsc)
                        {
                            orderBy = salesStatus.GetOrderBy<GiftModel>((IQueryable<GiftModel> o) =>
                                from d in o
                                orderby d.RealSales descending, d.Id descending
                                select d);
                            break;
                        }
                        else
                        {
                            orderBy = salesStatus.GetOrderBy<GiftModel>((IQueryable<GiftModel> o) =>
                                from d in o
                                orderby d.RealSales, d.Id descending
                                select d);
                            break;
                        }
                    }
                default:
                    {
                        orderBy = salesStatus.GetOrderBy<GiftModel>((IQueryable<GiftModel> o) =>
                            from d in o
                            orderby d.Sequence, d.Id descending
                            select d);
                        break;
                    }
            }
            int num = 0;
            salesStatus = salesStatus.GetPage(out num, query.PageNo, query.PageSize, orderBy);
            pageModel.Models = salesStatus;
            pageModel.Total = num;
            return pageModel;
        }

        private string HTMLProcess(string content, string path)
        {
            string str = Path.Combine(path, "Details").Replace("\\", "/");
            string mapPath = IOHelper.GetMapPath(str);
            string str1 = Path.Combine(path, "Temp").Replace("\\", "/");
            string mapPath1 = IOHelper.GetMapPath(str1);
            try
            {
                string str2 = str;
                string mapPath2 = IOHelper.GetMapPath(str2);
                content = HtmlContentHelper.TransferToLocalImage(content, IOHelper.GetMapPath("/"), mapPath2, string.Concat(str2, "/"));
                content = HtmlContentHelper.RemoveScriptsAndStyles(content);
                if (Directory.Exists(mapPath1))
                {
                    Directory.Delete(mapPath1, true);
                }
            }
            catch
            {
                if (Directory.Exists(mapPath1))
                {
                    string[] files = Directory.GetFiles(mapPath1);
                    for (int i = 0; i < files.Length; i++)
                    {
                        string str3 = files[i];
                        File.Copy(str3, Path.Combine(mapPath, Path.GetFileName(str3)), true);
                    }
                    Directory.Delete(mapPath1, true);
                }
            }
            return content;
        }

        public void UpdateGift(GiftInfo model)
        {
            if (string.IsNullOrWhiteSpace(model.ImagePath))
            {
                model.ImagePath = string.Format("/Storage/Gift/{0}", model.Id);
            }
            model.Description = HTMLProcess(model.Description, model.ImagePath);
            context.Entry<GiftInfo>(model).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void UpdateSequence(long id, int sequence)
        {
            GiftInfo giftInfo = context.GiftInfo.FindById<GiftInfo>(id);
            if (giftInfo == null)
            {
                throw new HimallException(string.Concat("未找到id为", id, "的礼品"));
            }
            giftInfo.Sequence = sequence;
            context.SaveChanges();
        }
    }
}