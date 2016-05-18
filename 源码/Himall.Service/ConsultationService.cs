using Himall.Core;
using Himall.Entity;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Himall.Service
{
	public class ConsultationService : ServiceBase, IConsultationService, IService, IDisposable
	{
		public ConsultationService()
		{
		}

		public void AddConsultation(ProductConsultationInfo model)
		{
			ProductInfo productInfo = (
				from a in context.ProductInfo
				where a.Id == model.ProductId && (int)a.SaleStatus != 4
				select a).FirstOrDefault();
			if (productInfo == null)
			{
				throw new HimallException("咨询的商品不存在，或者已删除");
			}
			model.ShopId = productInfo.ShopId;
			model.ShopName = (
				from a in context.ShopInfo
				where a.Id == model.ShopId
				select a into p
				select p.ShopName).FirstOrDefault();
            context.ProductConsultationInfo.Add(model);
            context.SaveChanges();
		}

		public void DeleteConsultation(long id)
		{
            context.ProductConsultationInfo.Remove(new object[] { id });
            context.SaveChanges();
		}

		public ProductConsultationInfo GetConsultation(long id)
		{
			return context.ProductConsultationInfo.FindById<ProductConsultationInfo>(id);
		}

		public PageModel<ProductConsultationInfo> GetConsultations(ConsultationQuery query)
		{
			int num = 0;
			IQueryable<ProductConsultationInfo> shopID = context.ProductConsultationInfo.Include<ProductConsultationInfo, ProductInfo>((ProductConsultationInfo a) => a.ProductInfo).AsQueryable<ProductConsultationInfo>();
			if (query.IsReply.HasValue)
			{
				shopID = 
					from item in shopID
					where (query.IsReply.Value ? item.ReplyDate.HasValue : !item.ReplyDate.HasValue)
					select item;
			}
			if (!string.IsNullOrWhiteSpace(query.KeyWords))
			{
				shopID = 
					from item in shopID
					where item.ConsultationContent.Contains(query.KeyWords)
					select item;
			}
			if (query.ShopID > 0)
			{
				shopID = 
					from item in shopID
					where query.ShopID == item.ShopId
					select item;
			}
			if (query.ProductID > 0)
			{
				shopID = 
					from item in shopID
					where query.ProductID == item.ProductId
					select item;
			}
			if (query.UserID > 0)
			{
				shopID = 
					from item in shopID
					where query.UserID == item.UserId
					select item;
			}
			shopID = shopID.GetPage(out num, query.PageNo, query.PageSize, null);
			return new PageModel<ProductConsultationInfo>()
			{
				Models = shopID,
				Total = num
			};
		}

		public IQueryable<ProductConsultationInfo> GetConsultations(long pid)
		{
			return context.ProductConsultationInfo.FindBy((ProductConsultationInfo c) => c.ProductId.Equals(pid));
		}

		public void ReplyConsultation(long id, string replyContent, long shopId)
		{
			ProductConsultationInfo nullable = context.ProductConsultationInfo.FindBy((ProductConsultationInfo item) => item.Id == id && item.ShopId == shopId).FirstOrDefault();
			if (shopId == 0 || nullable == null)
			{
				throw new HimallException("不存在该商品评论");
			}
			nullable.ReplyContent = replyContent;
			nullable.ReplyDate = new DateTime?(DateTime.Now);
            context.SaveChanges();
		}
	}
}