using Himall.Entity;
using Himall.IServices;
using Himall.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace Himall.Service
{
	public class FreightTemplateService : ServiceBase, IFreightTemplateService, IService, IDisposable
	{
		public FreightTemplateService()
		{
		}

		public void DeleteFreightTemplate(long TemplateId)
		{
			using (TransactionScope transactionScope = new TransactionScope())
			{
                context.FreightTemplateInfo.OrderBy((FreightTemplateInfo e) => e.Id == TemplateId);
                context.FreightAreaContentInfo.OrderBy((FreightAreaContentInfo e) => e.FreightTemplateId == TemplateId);
                context.SaveChanges();
				transactionScope.Complete();
			}
		}

		public IQueryable<FreightAreaContentInfo> GetFreightAreaContent(long TemplateId)
		{
			return 
				from e in context.FreightAreaContentInfo
				where e.FreightTemplateId == TemplateId
				select e;
		}

		public FreightTemplateInfo GetFreightTemplate(long TemplateId)
		{
			return (
				from e in context.FreightTemplateInfo
				where e.Id == TemplateId
				select e).FirstOrDefault();
		}

		public IQueryable<ProductInfo> GetProductUseFreightTemp(long TemplateId)
		{
			return 
				from item in context.ProductInfo
				where item.FreightTemplateId == TemplateId && (int)item.SaleStatus != 4
				select item;
		}

		public IQueryable<FreightTemplateInfo> GetShopFreightTemplate(long ShopID)
		{
			return 
				from item in context.FreightTemplateInfo
				where item.ShopID == ShopID
				select item;
		}

		public void UpdateFreightTemplate(FreightTemplateInfo templateInfo)
		{
			FreightTemplateInfo name;
			if (templateInfo.Id == 0)
			{
				name = context.FreightTemplateInfo.Add(templateInfo);
                context.SaveChanges();
				return;
			}
			name = (
				from e in context.FreightTemplateInfo
				where e.Id == templateInfo.Id
				select e).FirstOrDefault();
			name.Name = templateInfo.Name;
			name.IsFree = templateInfo.IsFree;
			name.ValuationMethod = templateInfo.ValuationMethod;
			name.ShopID = templateInfo.ShopID;
			name.SourceAddress = templateInfo.SourceAddress;
			name.SendTime = templateInfo.SendTime;
			using (TransactionScope transactionScope = new TransactionScope())
			{
                context.FreightAreaContentInfo.RemoveRange(
					from e in context.FreightAreaContentInfo
					where e.FreightTemplateId == name.Id
					select e);
                context.SaveChanges();
				if (name.IsFree == FreightTemplateInfo.FreightTemplateType.SelfDefine)
				{
					templateInfo.Himall_FreightAreaContent.ToList().ForEach((FreightAreaContentInfo e) => {
						FreightAreaContentInfo freightAreaContentInfo = new FreightAreaContentInfo()
						{
							AreaContent = e.AreaContent,
							FirstUnit = e.FirstUnit,
							FirstUnitMonry = e.FirstUnitMonry,
							AccumulationUnit = e.AccumulationUnit,
							AccumulationUnitMoney = e.AccumulationUnitMoney,
							IsDefault = e.IsDefault,
							FreightTemplateId = name.Id
						};
                        context.FreightAreaContentInfo.Add(freightAreaContentInfo);
					});
				}
                context.SaveChanges();
				transactionScope.Complete();
			}
		}
	}
}