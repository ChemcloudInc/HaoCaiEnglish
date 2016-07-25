using Himall.Entity;
using Himall.Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace Himall.Service.Job
{
    public class OrderCommentsJob : IJob
    {
        public OrderCommentsJob()
        {
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    Entities entity = new Entities();
                    List<OrderCommentsJob.OrderCommentsModel> list = (
                        from p in entity.OrderCommentInfo
                        group p by p.ShopId into g
                        select new OrderCommentsJob.OrderCommentsModel()
                        {
                            ShopId = g.Key,
                            AvgDeliveryMark = g.Average<OrderCommentInfo>((OrderCommentInfo p) => p.DeliveryMark),
                            AvgPackMark = g.Average<OrderCommentInfo>((OrderCommentInfo p) => p.PackMark),
                            AvgServiceMark = g.Average<OrderCommentInfo>((OrderCommentInfo p) => p.ServiceMark),
                            CategoryIds = entity.BusinessCategoryInfo.Where((BusinessCategoryInfo c) => c.ShopId == g.Key).Select<BusinessCategoryInfo, long>((BusinessCategoryInfo c) => c.CategoryId).ToList()
                        }).ToList<OrderCommentsJob.OrderCommentsModel>();
                    foreach (OrderCommentsJob.OrderCommentsModel orderCommentsModel in list)
                    {
                        List<OrderCommentsJob.OrderCommentsModel> orderCommentsModels = new List<OrderCommentsJob.OrderCommentsModel>();
                        foreach (long categoryId in orderCommentsModel.CategoryIds)
                        {
                            IEnumerable<OrderCommentsJob.OrderCommentsModel> orderCommentsModels1 =
                                from c in list
                                where c.CategoryIds.Contains(categoryId)
                                select c;
                            if (orderCommentsModels1 == null || orderCommentsModels1.Count<OrderCommentsJob.OrderCommentsModel>() <= 0)
                            {
                                continue;
                            }
                            orderCommentsModels.AddRange(orderCommentsModels1);
                        }
                        double num = (orderCommentsModels.Count != 0 ? orderCommentsModels.Average<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => c.AvgPackMark) : 0);
                        double num1 = (orderCommentsModels.Count != 0 ? orderCommentsModels.Average<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => c.AvgDeliveryMark) : 0);
                        double num2 = (orderCommentsModels.Count != 0 ? orderCommentsModels.Average<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => c.AvgServiceMark) : 0);
                        double num3 = (orderCommentsModels.Count != 0 ? list.Where<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => (
                            from o in orderCommentsModels
                            where o.ShopId == c.ShopId
                            select o).Count<OrderCommentsJob.OrderCommentsModel>() > 0).Max<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => c.AvgPackMark) : 0);
                        double num4 = (orderCommentsModels.Count != 0 ? list.Where<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => (
                            from o in orderCommentsModels
                            where o.ShopId == c.ShopId
                            select o).Count<OrderCommentsJob.OrderCommentsModel>() > 0).Min<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => c.AvgPackMark) : 0);
                        double num5 = (orderCommentsModels.Count != 0 ? list.Where<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => (
                            from o in orderCommentsModels
                            where o.ShopId == c.ShopId
                            select o).Count<OrderCommentsJob.OrderCommentsModel>() > 0).Max<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => c.AvgServiceMark) : 0);
                        double num6 = (orderCommentsModels.Count != 0 ? list.Where<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => (
                            from o in orderCommentsModels
                            where o.ShopId == c.ShopId
                            select o).Count<OrderCommentsJob.OrderCommentsModel>() > 0).Min<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => c.AvgServiceMark) : 0);
                        double num7 = (orderCommentsModels.Count != 0 ? list.Where<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => (
                            from o in orderCommentsModels
                            where o.ShopId == c.ShopId
                            select o).Count<OrderCommentsJob.OrderCommentsModel>() > 0).Max<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => c.AvgDeliveryMark) : 0);
                        double num8 = (orderCommentsModels.Count != 0 ? list.Where<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => (
                            from o in orderCommentsModels
                            where o.ShopId == c.ShopId
                            select o).Count<OrderCommentsJob.OrderCommentsModel>() > 0).Min<OrderCommentsJob.OrderCommentsModel>((OrderCommentsJob.OrderCommentsModel c) => c.AvgDeliveryMark) : 0);
                        Entities entity1 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.ProductAndDescription,
                            CommentValue = (decimal)orderCommentsModel.AvgPackMark
                        };
                        Save(entity1, statisticOrderCommentsInfo);
                        Entities entity2 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo1 = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.ProductAndDescriptionPeer,
                            CommentValue = (decimal)num
                        };
                        Save(entity2, statisticOrderCommentsInfo1);
                        Entities entity3 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo2 = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.ProductAndDescriptionMax,
                            CommentValue = (decimal)num3
                        };
                        Save(entity3, statisticOrderCommentsInfo2);
                        Entities entity4 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo3 = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.ProductAndDescriptionMin,
                            CommentValue = (decimal)num4
                        };
                        Save(entity4, statisticOrderCommentsInfo3);
                        Entities entity5 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo4 = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.SellerServiceAttitude,
                            CommentValue = (decimal)orderCommentsModel.AvgServiceMark
                        };
                        Save(entity5, statisticOrderCommentsInfo4);
                        Entities entity6 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo5 = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.SellerServiceAttitudePeer,
                            CommentValue = (decimal)num2
                        };
                        Save(entity6, statisticOrderCommentsInfo5);
                        Entities entity7 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo6 = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.SellerServiceAttitudeMax,
                            CommentValue = (decimal)num5
                        };
                        Save(entity7, statisticOrderCommentsInfo6);
                        Entities entity8 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo7 = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.SellerServiceAttitudeMin,
                            CommentValue = (decimal)num6
                        };
                        Save(entity8, statisticOrderCommentsInfo7);
                        Entities entity9 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo8 = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.SellerDeliverySpeed,
                            CommentValue = (decimal)orderCommentsModel.AvgDeliveryMark
                        };
                        Save(entity9, statisticOrderCommentsInfo8);
                        Entities entity10 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo9 = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.SellerDeliverySpeedPeer,
                            CommentValue = (decimal)num1
                        };
                        Save(entity10, statisticOrderCommentsInfo9);
                        Entities entity11 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo10 = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.SellerDeliverySpeedMax,
                            CommentValue = (decimal)num7
                        };
                        Save(entity11, statisticOrderCommentsInfo10);
                        Entities entity12 = entity;
                        StatisticOrderCommentsInfo statisticOrderCommentsInfo11 = new StatisticOrderCommentsInfo()
                        {
                            ShopId = orderCommentsModel.ShopId,
                            CommentKey = StatisticOrderCommentsInfo.EnumCommentKey.SellerDeliverySpeedMin,
                            CommentValue = (decimal)num8
                        };
                        Save(entity12, statisticOrderCommentsInfo11);
                    }
                    entity.SaveChanges();
                    transactionScope.Complete();
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        private void Save(Entities entity, StatisticOrderCommentsInfo comment)
        {
            StatisticOrderCommentsInfo commentValue = (
                from c in entity.StatisticOrderCommentsInfo
                where c.ShopId == comment.ShopId && (int)c.CommentKey == (int)comment.CommentKey
                select c).FirstOrDefault();
            if (commentValue != null)
            {
                commentValue.CommentValue = comment.CommentValue;
            }
            else
            {
                ShopInfo shopInfo = (
                    from c in entity.ShopInfo
                    where c.Id == comment.ShopId
                    select c).FirstOrDefault();
                if (shopInfo != null)
                {
                    comment.Himall_Shops = shopInfo;
                    entity.StatisticOrderCommentsInfo.Add(comment);
                    return;
                }
            }
        }

        public class OrderCommentsModel
        {
            public double AvgDeliveryMark
            {
                get;
                set;
            }

            public double AvgPackMark
            {
                get;
                set;
            }

            public double AvgServiceMark
            {
                get;
                set;
            }

            public IList<long> CategoryIds
            {
                get;
                set;
            }

            public long ShopId
            {
                get;
                set;
            }

            public OrderCommentsModel()
            {
            }
        }
    }
}